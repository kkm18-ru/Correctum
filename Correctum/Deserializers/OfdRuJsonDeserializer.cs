using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Utils;
using Correctum.Attrubutes;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Correctum.Deserializers;

[DeserializerInfo("ОФД.ру (JSON)")]
public partial class OfdRuJsonDeserializer : IDeserializer
{
    public static Receipt Deserialize(ReadOnlySpan<char> text)
    {
        var doc = JsonSerializer.Deserialize(text, OfdRuJsonSerializeContext.Default.DocumentDto);
        return OfdRuJsonHelper.ToReceipt(doc.Receipt);
    }

    [JsonSerializable(typeof(DocumentDto))]
    private sealed partial class OfdRuJsonSerializeContext : JsonSerializerContext;

    private readonly struct DocumentDto
    {
        [JsonPropertyName("Document")] public ReceiptDto Receipt { get; init; }
    }

    private readonly struct ReceiptDto
    {
        [JsonPropertyName("DecimalFiscalSign")] public string? Sign { get; init; }
        [JsonPropertyName("KKT_RegNumber")] public string? KktReg { get; init; }
        [JsonPropertyName("FN_FactoryNumber")] public string? Drive { get; init; }
        [JsonPropertyName("Document_Number")] public uint? Doc { get; init; }
        [JsonPropertyName("Number")] public uint? DocSession { get; init; }
        [JsonPropertyName("ShiftNumber")] public uint? Session { get; init; }
        [JsonPropertyName("DateTime")] public string? DateTime { get; init; }
        [JsonPropertyName("Operator")] public string? Cashier { get; init; }
        [JsonPropertyName("OperatorInn")] public string? CashierInn { get; init; } // todo need check
        [JsonPropertyName("OperationType")] public int? Type { get; init; }
        [JsonPropertyName("TaxationType")] public int? Taxation { get; init; }
        [JsonPropertyName("Amount_Total")] public ulong? Total { get; init; }
        [JsonPropertyName("Amount_Cash")] public ulong? Cash { get; init; }
        [JsonPropertyName("Amount_ECash")] public ulong? NonCash { get; init; }
        [JsonPropertyName("Amount_Advance")] public ulong? Advance { get; init; }
        [JsonPropertyName("Amount_Loan")] public ulong? Credit { get; init; }
        [JsonPropertyName("Amount_Granting")] public ulong? Provision { get; init; }
        [JsonPropertyName("Items")] public PositionDto[]? Items { get; init; }
    }

    private readonly struct PositionDto
    {
        [JsonPropertyName("Name")] public string? Name { get; init; }
        [JsonPropertyName("Price")] public ulong? Price { get; init; }
        [JsonPropertyName("Quantity")] public decimal? Quantity { get; init; }
        [JsonPropertyName("Total")] public ulong? Cost { get; init; }
        [JsonPropertyName("ProductUnitOfMeasure")] public int? Unit { get; init; }
        [JsonPropertyName("CalculationMethod")] public int? CalcMethod { get; init; }
        [JsonPropertyName("NDS_Rate")] public int? Tax { get; init; }
        [JsonPropertyName("SubjectType")] public int? Type { get; init; }
    }

    private static class OfdRuJsonHelper
    {
        private static Position ToPosition(PositionDto dto) => new()
        {
            Name = dto.Name,
            Price = dto.Price,
            Quantity = dto.Quantity is null ? null : (ulong)Math.Round(dto.Quantity.Value * 1000),
            Cost = dto.Cost,
            Unit = (UnitOfMeasure?)dto.Unit,
            CalcMethod = (CalculationMethod?)dto.CalcMethod,
            TaxRate = NDSRate.FromFns(dto.Tax),
            Type = (PositionType?)dto.Type,
        };

        public static Receipt ToReceipt(ReceiptDto dto) => new()
        {
            Items = dto.Items is null ? null : [..dto.Items.Select(ToPosition)],
            Payment = new()
            {
                CashSum = dto.Cash,
                NonCashSum = dto.NonCash,
                AdvanceSum = dto.Advance,
                CreditSum = dto.Credit,
                ProvisionSum = dto.Provision,
            },
            TotalSum = dto.Total,
            DocSign = dto.Sign is null ? null : FiscalSign.Parse(dto.Sign),
            RegId = dto.KktReg,
            StorageId = dto.Drive,
            DocNumber = dto.Doc,
            SessionId = dto.Session,
            DocInSession = dto.DocSession,
            Type = (ReceiptType?)dto.Type,
            Created = dto.DateTime is null ? null : DateTime.ParseExact(dto.DateTime, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
            TaxationSystem = (TaxationSystems?)dto.Taxation,
            // todo BuyerContact
            Cashier = dto.Cashier,
            CashierInn = dto.CashierInn is null ? null : Inn.Parse(dto.CashierInn)
        };
    }
}
