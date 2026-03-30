using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal partial class OfdYaSheet : ImporterBase<OfdYaSheet, OfdYaSheet.View>, INotifyPropertyChanged
{
    public string Path
    {
        get; set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
        }
    } = string.Empty;

    public override string Name => "ОФД-Я (xlsx)";

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

    public override ImportSession GenerateInfo() => 
        parseInfo = new(Name);


    public override async IAsyncEnumerable<Receipt> Import(IProgress<long> progress, CancellationToken cancellationToken)
    {
        var indexes = new int[25]
        {
            -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1,
        };
        Receipt current = default;
        
        await using var stream = new FileStream(Path, FileMode.Open);
        stream.Position = 0;
        using var workbook = new XSSFWorkbook(stream);
        var sheet = workbook.GetSheetAt(0);
        var header = sheet.GetRow(1);
        var cellCount = header.LastCellNum;
        for (var i = 0; i < cellCount; i++)
        {
            var cell = header.GetCell(i);
            if (cell is not null)
            {
                indexes[cell.ToString() switch
                {
                    "Сумма ФД, руб" => 0,
                    "Сумма наличные, руб" => 1,
                    "Сумма электронно, руб" => 2,
                    "Сумма предоплатой (аванс)" => 3,
                    "Сумма постоплатой (в кредит)" => 4,
                    "Сумма встречным предоставлением" => 5,
                    "Признак способа расчета" => 6,
                    "Признак предмета расчета" => 7,
                    "Наименование предмета расчета" => 8,
                    "Цена за единицу с учетом скидок и наценок" => 9,
                    "Количество" => 10,
                    "Ставка НДС" => 11,
                    "Стоимость товара с учетом скидок и наценок" => 12,
                    "ИНН" => 13,
                    "Регистрационный номер ККТ" => 14,
                    "Заводской номер ФН" => 15,
                    "Применяемая система налогообложения" => 16,
                    "Тип фискального документа" => 17,
                    "Признак расчета" => 18,
                    "Номер смены" => 19,
                    "Номер ФД за смену" => 20,
                    "Порядковый номер ФД" => 21,
                    "Дата и время ФД" => 22,
                    "Единица измерений товара" => 23,
                    "Фискальный признак документа" => 24,
                    _ => throw new NotImplementedException()
                }] = i;
            }
        }
        if (indexes.Any(i => i == -1))
            throw new FormatException("Некорректный формат таблицы");
        var rowCount = sheet.LastRowNum;
        await Task.Run(() => progress.Report(-rowCount + 2), cancellationToken);
        for (var row = 2; row < rowCount; row++)
        {
            await Task.Run(() => progress.Report(row - 2), cancellationToken);
            var r = sheet.GetRow(row);
            if (current.StorageId == r.GetCell(indexes[15]).StringCellValue &&
                current.DocNumber == (uint)r.GetCell(indexes[21]).NumericCellValue)
            {
                var items = current.Items;
                items = [..items, new() {
                    Name = r.GetCell(indexes[8]).StringCellValue,
                        Type = r.GetCell(indexes[7]).StringCellValue switch
                        {
                            "Товар" => PositionType.Product,
                            "АТМ" => PositionType.ExciseMark,
                            "Подакцизный товар" => PositionType.Excise
                        },
                        Cost = (ulong)Math.Round(r.GetCell(indexes[12]).NumericCellValue*100),
                        PayMethod = r.GetCell(indexes[6]).StringCellValue switch
                        {
                            "ПОЛНЫЙ РАСЧЕТ" => PayMethod.FullPayment,
                        },
                        Price = (ulong)Math.Round(r.GetCell(indexes[9]).NumericCellValue*100),
                        Quantity = (ulong)Math.Round(r.GetCell(indexes[10]).NumericCellValue*1000),
                        TaxRate = r.GetCell(indexes[11]).StringCellValue switch
                        {
                            "НДС не облагается" => TaxRate.Without
                        },
                        Unit = r.GetCell(indexes[23]).StringCellValue switch
                        {
                            "шт." => PositionUnit.Piece
                        },
                }];
                current.Items = items;
            }
            else
            {
                if (current.ParseId != Guid.Empty)
                    yield return current;
                current = new Receipt
                {
                    ParseId = parseInfo.Id,
                    Created = DateTime.ParseExact(r.GetCell(indexes[22]).StringCellValue, "dd-MM-yyyy HH:mm:ss", null),
                    DocInSession = (uint)r.GetCell(indexes[20]).NumericCellValue,
                    DocNumber = (uint)r.GetCell(indexes[21]).NumericCellValue,
                    DocSign = (uint)r.GetCell(indexes[24]).NumericCellValue,
                    Items = [new Position() {
                        Name = r.GetCell(indexes[8]).StringCellValue,
                        Type = r.GetCell(indexes[7]).StringCellValue switch
                        {
                            "Товар" => PositionType.Product,
                            "АТМ" => PositionType.ExciseMark,
                            "Подакцизный товар" => PositionType.Excise
                        },
                        Cost = (ulong)Math.Round(r.GetCell(indexes[12]).NumericCellValue*100),
                        PayMethod = r.GetCell(indexes[6]).StringCellValue switch
                        {
                            "ПОЛНЫЙ РАСЧЕТ" => PayMethod.FullPayment,
                        },
                        Price = (ulong)Math.Round(r.GetCell(indexes[9]).NumericCellValue*100),
                        Quantity = (ulong)Math.Round(r.GetCell(indexes[10]).NumericCellValue*1000),
                        TaxRate = r.GetCell(indexes[11]).StringCellValue switch
                        {
                            "НДС не облагается" => TaxRate.Without
                        },
                        Unit = r.GetCell(indexes[23]).StringCellValue switch
                        {
                            "шт." => PositionUnit.Piece
                        },
                    }],
                    RegId = r.GetCell(indexes[14]).StringCellValue,
                    SessionId = (uint)r.GetCell(indexes[19]).NumericCellValue,
                    StorageId = r.GetCell(indexes[15]).StringCellValue,
                    TaxationSystem = r.GetCell(indexes[16]).StringCellValue switch
                    {
                        "Упрощенная доход" => TaxationSystem.USNIncome
                    },
                    Type = r.GetCell(indexes[18]).StringCellValue switch
                    {
                        "Приход" => Operation.Income
                    },
                    Payment = new()
                    {
                        Cash = (ulong)Math.Round(r.GetCell(indexes[1]).NumericCellValue * 100),
                        ECash = (ulong)Math.Round(r.GetCell(indexes[2]).NumericCellValue * 100),
                        Pre = (ulong)Math.Round(r.GetCell(indexes[3]).NumericCellValue * 100),
                        Post = (ulong)Math.Round(r.GetCell(indexes[4]).NumericCellValue * 100),
                        Provision = (ulong)Math.Round(r.GetCell(indexes[5]).NumericCellValue * 100),
                    },
                    TotalSum = (ulong)Math.Round(r.GetCell(indexes[0]).NumericCellValue * 100),
                };
            }
        }
    }
}
