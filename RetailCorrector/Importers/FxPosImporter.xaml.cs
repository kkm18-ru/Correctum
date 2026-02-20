using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.Data.Sqlite;
using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

[ImporterName("FxPOS")]
public sealed partial class FxPosImporter : ImporterBase
{   
    [ImporterProperty("Путь к файлу")]
    public string FilePath { get; set => SetField(ref field, value); } = "";

    public FxPosImporter()
    {
        InitializeComponent();
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, Guid sessionId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using (var conn = new SqliteConnection($"Data Source={FilePath};Mode=ReadOnly"))
        {
            var count = await conn.ExecuteScalarAsync<int>(COUNT_RECEIPTS);
            progress.ReportMaxValue(count);
            for (var i = 0; i < count; i += Configuration.Instance.Sqlite.Limit)
            {
                progress.ReportCurrValue(i);
                cancellationToken.ThrowIfCancellationRequested();
                var docs = (await Task.Run(() => conn.QueryAsync<Document>(GET_RECEIPTS, new { limit = Configuration.Instance.Sqlite.Limit, offset = i }), cancellationToken)).ToArray();
                foreach (var doc in docs)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return new Receipt
                    {
                        ImportSessionId = sessionId,
                        Items = [.. (await Task.Run(() => conn.QueryAsync<InnerPosition>(GET_POSITIONS, doc), cancellationToken)).Select(i => (Position)i)],
                        Created = doc.Date,
                        DocSign = doc.Sign,
                        TotalSum = doc.Total,
                        Payment = await Task.Run(() => conn.QuerySingleAsync<Payment>(GET_PAYMENT, doc), cancellationToken),
                        Type = doc.Type switch
                        {
                            0 or 19 => Operation.Income,
                            1 => Operation.RefundIncome,
                            25 or 20 => Operation.Outcome,
                            26 => Operation.RefundOutcome
                        }
                    };
                }
            }
        }
        SqliteConnection.ClearAllPools();
    }
    
    const string COUNT_RECEIPTS = "select count(ID) from DOCUMENTS where DOCUMENTSTATE = 1 and ECRDOCUMENTTYPE = 0;";
    const string GET_RECEIPTS = """
                                select 
                                    d.ID as Id, 
                                    d.CLOSEDATETIME as Date, 
                                    cast(d.TOTAL/100 as integer) as Total,
                                    cast(tr.WAREMARK as integer) as Sign,
                                    cast(d.ECRDOCUMENTTYPE as integer) as Type
                                from DOCUMENTS as d inner join TRANSACTIONS as tr on tr.DOCUMENTID = d.ID
                                where d.DOCUMENTSTATE = 1 and d.ECRDOCUMENTTYPE = 0 and tr.TRTYPE = 45 
                                limit @limit offset @offset;
                                """;
    const string GET_POSITIONS =
        """
        select
            cast(w.NAME as TEXT) as Name, 
            cast(w.PAYMENT_TYPE as INTEGER) as PayMethod,
            cast(w.ITEM_TYPE as INTEGER) as Type,
            (select cast(VALUE/1000000 as INTEGER) from TAXRATE where tr.TAXRATE_CODE = CODE) as TaxRate,
            cast(tr.PRICE/100 as INTEGER) as Price,
            cast(tr.QUANTITY/1000 as INTEGER) as Quantity,    
            cast(w.MEASURE as INTEGER) as Unit,
            cast(tr.TOTAL/100 as INTEGER) as Cost,
            cast(w.MARKING as BOOLEAN) as HasMark
        from TRANSACTIONS as tr inner join WARE as w on w.CODE = tr.WARECODE
        where tr.TRTYPE in (6,16) and tr.DOCUMENTID = @Id;
        """;
    const string GET_PAYMENT =
        """  
        SELECT
            SUM(CASE WHEN p.ECRPAYMENT = 0 THEN tr.total/100 ELSE 0 END) AS Cash,
            SUM(CASE WHEN p.ECRPAYMENT = 1 THEN tr.total/100 ELSE 0 END) AS ECash,
            SUM(CASE WHEN p.ECRPAYMENT = 2 THEN tr.total/100 ELSE 0 END) AS Pre,
            SUM(CASE WHEN p.ECRPAYMENT = 3 THEN tr.total/100 ELSE 0 END) AS Post,
            SUM(CASE WHEN p.ECRPAYMENT = 4 THEN tr.total/100 ELSE 0 END) AS Provision
        FROM TRANSACTIONS AS tr INNER JOIN PAYMENT AS p ON tr.warecode = p.CODE
        WHERE tr.TRTYPE IN (40, 41) AND tr.documentid = @Id;
        """;
}

file struct Document
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public ulong Total { get; set; }
    public uint Sign { get; set; }
    public int Type { get; set; }
}

file struct InnerPosition
{
    public string Name { get; set; }
    public int PayMethod { get; set; }
    public int Type { get; set; }
    public int TaxRate { get; set; }
    public ulong Price { get; set; }
    public ulong Quantity { get; set; }
    public int Unit { get; set; }
    public ulong Cost { get; set; }
    public bool HasMark { get; set; }

    public static explicit operator Position(InnerPosition raw) => new()
    {
        Name = raw.Name,
        Price = raw.Price,
        Quantity = raw.Quantity,
        Cost = raw.Cost,
        Unit = raw.Unit switch
        {
            0 => PositionUnit.Piece,
            1 => PositionUnit.Gram,
            2 => PositionUnit.Kilogram,
            3 => PositionUnit.Ton,
            4 => PositionUnit.Centimeter,
            5 => PositionUnit.Decimeter,
            6 => PositionUnit.Meter,
            7 => PositionUnit.SquareCentimeter,
            8 => PositionUnit.SquareDecimeter,
            9 => PositionUnit.SquareMeter,
            10 => PositionUnit.Milliliter,
            11 => PositionUnit.Liter,
            12 => PositionUnit.CubicMeter,
            13 => PositionUnit.KilowattHour,
            14 => PositionUnit.Gigacalories,
            15 => PositionUnit.Days,
            16 => PositionUnit.Hours,
            17 => PositionUnit.Minutes,
            18 => PositionUnit.Seconds,
            19 => PositionUnit.Kilobyte,
            20 => PositionUnit.Megabyte,
            21 => PositionUnit.Gigabyte,
            22 => PositionUnit.Terabyte,
            23 => PositionUnit.Other,
        },
        PayMethod = (PayMethod)raw.PayMethod,
        Type = (raw.Type, raw.HasMark) switch
        {
            (1, false) => PositionType.Product,
            (1, true) => PositionType.Mark,
            (2, false) => PositionType.Excise,
            (2, true) => PositionType.ExciseMark,
            (3, _) => PositionType.Work,
            (4, _) => PositionType.Service,
            (10, _) => PositionType.Payment,
            (12, _) => PositionType.Other,
            (13, _) => PositionType.Agent,
            (14, _) => PositionType.Lottery,
            (15, _) => PositionType.IntellectualProperty,
            (16, _) => PositionType.Forfeit,
            (17, _) => PositionType.ResortFee,
            (18, _) => PositionType.FromBankAgent,
        },
        TaxRate = raw.TaxRate switch
        {
            0 => Enums.TaxRate.Without,
            5 => Enums.TaxRate.Tax5,
            7 => Enums.TaxRate.Tax7,
            10 => Enums.TaxRate.Tax10,
            18 => Enums.TaxRate.Tax18,
            20 => Enums.TaxRate.Tax20,
            22 => Enums.TaxRate.Tax22,
        }
    };
}