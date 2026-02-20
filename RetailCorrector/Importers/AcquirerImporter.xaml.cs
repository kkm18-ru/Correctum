using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

public sealed partial class AcquirerImporter : ImporterBase
{
    public new static string FormName { get; } = "Отчет эквайринга";
    
    public string FilePath { get; set => SetField(ref field, value); } = "";
    public char Separator { get; set => SetField(ref field, value); } = ';';
    public bool HasHeaders { get; set => SetField(ref field, value); } = true;
    public int OperationReceipt { get; set => SetField(ref field, value); } = -1;
    public int DateReceipt { get; set => SetField(ref field, value); } = -1;
    public int TotalReceipt { get; set => SetField(ref field, value); } = -1;
    public string DateFormat { get; set => SetField(ref field, value); } = "yyyy-MM-dd HH:mm:ss";
    public string ItemName { get; set => SetField(ref field, value); } = "";
    public string IncomePattern { get; set => SetField(ref field, value); } = "Поступление";
    public string RefundPattern { get; set => SetField(ref field, value); } = "Возврат";
    public TaxRate ItemVat { get; set => SetField(ref field, value); } = TaxRate.Without;
    public ObservableCollection<string> Headers { get; } = [];
    
    public AcquirerImporter()
    {
        InitializeComponent();
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var lines = await File.ReadAllLinesAsync(FilePath, cancellationToken);
        for (var i = (HasHeaders ? 1u : 0); i < lines.Length; i++)
        {
            var columns = lines[i].Split(Separator);
            var sum = (ulong)(double.Parse(columns[TotalReceipt]) * 100);
            var operation = columns[OperationReceipt].Trim('"');
            yield return new Receipt
            {
                ParseId = Guid.Empty, // todo
                DocInSession = 0,
                RegId = 0.ToString("D16"),
                StorageId = 0.ToString("D16"),
                SessionId = 0,
                EditsCount = 0,
                DocNumber = i,
                Created = DateTime.ParseExact(columns[DateReceipt].Trim('"'), DateFormat, null),
                TotalSum = sum,
                BuyerContact = columns[3],
                Type = operation == IncomePattern ? Operation.Income : operation == RefundPattern ? Operation.RefundIncome : 0,
                Items = [
                    new Position
                    {
                        Name = ItemName,
                        Price = sum,
                        Quantity = 1000,
                        Cost = sum,
                        TaxRate = ItemVat,
                        Unit = PositionUnit.Piece,
                        PayMethod = PayMethod.FullPayment,
                        Type = PositionType.Product
                    }
                ],
                Payment = new Payment { ECash = sum },
                TaxationSystem = 0,
                DocSign = 0
            };
        }
    }

    private void ReadFirstLine(object sender, RoutedEventArgs e)
    {
        var line = File.ReadLine(FilePath);
        var columns = line.Split(Separator);
        Headers.Clear();
        for (var i = 0; i < columns.Length; i++)
        {
            var header = new StringBuilder(Helper.GenerateColumnName((uint)i + 1));
            if (HasHeaders) header.Append($" ({columns[i].Trim('"')})");
            Headers.Add(header.ToString());
        }
    }
}

file static class Helper
{
    public static string GenerateColumnName(uint value)
    {
        if (value == 0) return "";
        var power = value switch
        {
            1 => (int)value--,
            > 1 => (int)Math.Ceiling(Math.Log(value--, 26)),
            _ => throw new ArgumentException(null, nameof(value))
        };
        var builder = new StringBuilder(power);
        for (var i = power - 1; i >= 0; i--)
            builder.Append((char)((i == 0 ? 'A' : '@') + value / Math.Pow(26, i) % 26));
        return builder.ToString();
    }
} 