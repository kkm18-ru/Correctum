using System.Text.Json;
using Correctum.Core.Models;
using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace RetailCorrector.Parsers;

internal sealed class OfdRuJson : IParser
{
    public static OfdRuJson Instance => field ??= new OfdRuJson();
    public string Name => "ОФД.ру (JSON)";

    const string DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss";

    public bool Parse(string raw, out Receipt receipt)
    {
        try
        {
            var json = JsonSerializer.Deserialize<OfdRuJsonDocument>(raw);

            receipt = new()
            {
                DocSign = FiscalSign.Parse(json.Document.DecimalFiscalSign),
                RegId = json.Document.KKT_RegNumber,
                StorageId = json.Document.FN_FactoryNumber,
                DocNumber = json.Document.Document_Number,
                DocInSession = json.Document.Number,
                SessionId = json.Document.ShiftNumber,
                Created = DateTime.ParseExact(json.Document.DateTime, DATE_FORMAT,
                    Thread.CurrentThread.CurrentUICulture),
                Type = (ReceiptType)json.Document.OperationType,
                TotalSum = json.Document.Amount_Total,
                CashSum = json.Document.Amount_Cash,
                NonCashSum = json.Document.Amount_ECash,
                AdvanceSum = json.Document.Amount_Advance,
                CreditSum = json.Document.Amount_Loan,
                ProvisionSum = json.Document.Amount_Granting,
                Items = new Position[json.Document.Items.Length]
            };
            for (var i = 0; i < receipt.Items.Length; i++)
                receipt.Items[i] = json.Document.Items[i];
            return true;
        }
        catch
        {
            receipt = default;
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
    public decimal Quantity { get; init; }
    public ulong Total { get; init; }
    public int ProductUnitOfMeasure { get; init; }
    public int CalculationMethod { get; init; }
    public int NDS_Rate { get; init; }
    public int SubjectType { get; init; }

    public static implicit operator Position(OfdRuJsonPosition json) => new()
    {
        Name = json.Name,
        Price = json.Price,
        Quantity = json.Quantity,
        Cost = json.Total,
        Unit = (UnitOfMeasure)json.ProductUnitOfMeasure,
        CalcMethod = (CalculationMethod)json.CalculationMethod,
        TaxRate = (NDSRate)json.NDS_Rate,
        Type = (PositionType)json.SubjectType,
    };
}
