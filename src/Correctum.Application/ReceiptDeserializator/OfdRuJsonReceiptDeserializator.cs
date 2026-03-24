using Correctum.Enums;
using Correctum.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Correctum.ReceiptDeserializator;

public sealed partial class OfdRuReceiptDeserializator : IReceiptDeserializator
{
    /// <inheritdoc/>
    public Receipt Deserialize(ReadOnlySpan<char> text) =>
        JsonSerializer.Deserialize(text, JsonDocumentContext.Default.OfdRuJsonDocument).Document.ToModel();


    [JsonSerializable(typeof(OfdRuJsonDocument))]
    private partial class JsonDocumentContext : JsonSerializerContext;

    private readonly struct OfdRuJsonDocument
    {
        public OfdRuJsonReceipt Document { get; init; }
    }

    private readonly struct OfdRuJsonReceipt
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

        public Receipt ToModel() => new()
        {
            FiscalSign = uint.Parse(DecimalFiscalSign),
            RegNumber = KKT_RegNumber,
            DriveSerial = FN_FactoryNumber,
            Number = Document_Number,
            NumberInSession = Number,
            SessionNumber = ShiftNumber,
            DateTime = System.DateTime.ParseExact(DateTime, "yyyy-MM-ddTHH:mm:ss", 
                Thread.CurrentThread.CurrentUICulture),
            Type = (ReceiptType)OperationType,
            TotalSum = Amount_Total,
            CashSum = Amount_Cash,
            NonCashSum = Amount_ECash,
            PrepaidSum = Amount_Advance,
            CreditSum = Amount_Loan,
            OtherSum = Amount_Granting,
            Items = [.. Items.Select(i => i.ToModel())]
        };
    }

    private readonly struct OfdRuJsonPosition
    {
        public string Name { get; init; }
        public ulong Price { get; init; }
        public double Quantity { get; init; }
        public ulong Total { get; init; }
        public int ProductUnitOfMeasure { get; init; }
        public int CalculationMethod { get; init; }
        public int NDS_Rate { get; init; }
        public int SubjectType { get; init; }

        public Position ToModel() => new()
        {
            Name = Name,
            Price = Price,
            Quantity = (ulong)(Quantity * 1000),
            Cost = Total,
            Unit = (PositionUnit)ProductUnitOfMeasure,
            CalculationMethod = (PositionCalculationMethod)CalculationMethod,
            TaxRate = (TaxRate)NDS_Rate,
            SubjectType = (PositionType)SubjectType,
        };
    }
}
