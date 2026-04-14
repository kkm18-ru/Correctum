using System.Text.Json;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.ValueObjects;

namespace RetailCorrector.Parsers;

internal sealed class OfdRuJson : IParser
{
    public static OfdRuJson Instance => field ??= new OfdRuJson();
    public string Name => "ОФД.ру (JSON)";

    const string DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss";

    public bool Parse(string raw, ref Receipt receipt)
    {
        try
        {
            var json = JsonSerializer.Deserialize<OfdRuJsonDocument>(raw);

            receipt.DocSign = FiscalSign.Parse(json.Document.DecimalFiscalSign);
            receipt.RegId = json.Document.KKT_RegNumber;
            receipt.StorageId = json.Document.FN_FactoryNumber;
            receipt.DocNumber = json.Document.Document_Number;
            receipt.DocInSession = json.Document.Number;
            receipt.SessionId = json.Document.ShiftNumber;
            receipt.Created = DateTime.ParseExact(json.Document.DateTime, DATE_FORMAT, 
                Thread.CurrentThread.CurrentUICulture);
            receipt.Type = (Operation)json.Document.OperationType;
            receipt.TotalSum = json.Document.Amount_Total;
            receipt.Payment = new Payment
            {
                Cash = json.Document.Amount_Cash,
                ECash = json.Document.Amount_ECash,
                Pre = json.Document.Amount_Advance,
                Post = json.Document.Amount_Loan,
                Provision = json.Document.Amount_Granting,
            };
            receipt.Items = new Position[json.Document.Items.Length];
            for (var i = 0; i < receipt.Items.Length; i++)
                receipt.Items[i] = json.Document.Items[i];
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    
}

file readonly struct OfdRuJsonDocument
{
    public OfdRuJsonReceipt Document { get; init; }
}

file readonly struct OfdRuJsonReceipt
{
    public string DecimalFiscalSign { get; init; }
    public string KKT_RegNumber { get; init; }
    public string FN_FactoryNumber { get; init; }
    public uint Document_Number { get; init; }
    public uint Number { get; init; }
    public uint ShiftNumber { get; init; }
    public string DateTime { get; init; }
    public int OperationType { get; init; }
    public ulong Amount_Total { get; init; }
    public ulong Amount_Cash { get; init; }
    public ulong Amount_ECash { get; init; }
    public ulong Amount_Advance { get; init; }
    public ulong Amount_Loan { get; init; }
    public ulong Amount_Granting { get; init; }
    public OfdRuJsonPosition[] Items { get; init; }
}

file readonly struct OfdRuJsonPosition
{
    public string Name { get; init; }
    public ulong Price { get; init; }
    public double Quantity { get; init; }
    public ulong Total { get; init; }
    public int ProductUnitOfMeasure { get; init; }
    public int CalculationMethod { get; init; }
    public int NDS_Rate { get; init; }
    public int SubjectType { get; init; }

    public static implicit operator Position(OfdRuJsonPosition json) => new()
    {
        Name = json.Name,
        Price = json.Price,
        Quantity = (ulong)(json.Quantity * 1000),
        Cost = json.Total,
        Unit = (PositionUnit)json.ProductUnitOfMeasure,
        PayMethod = (PayMethod)json.CalculationMethod,
        TaxRate = (TaxRate)json.NDS_Rate,
        Type = (PositionType)json.SubjectType,
    };
}
