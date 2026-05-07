using Correctum.Attributes;
using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Utils;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Correctum.Deserializers;


[DeserializerInfo("ОФД.ру (TLV)")]
public partial class OfdRuTlvDeserializer : IDeserializer
{
    public static Receipt Deserialize(ReadOnlySpan<char> text)
    {
        var doc = JsonSerializer.Deserialize(text, OfdRuTlvSerializeContext.Default.ReceiptDto);
        return OfdRuJsonHelper.ToReceipt(doc);
    }

    [JsonSerializable(typeof(ReceiptDto))]
    private sealed partial class OfdRuTlvSerializeContext : JsonSerializerContext;

    private readonly struct ReceiptDto
    {
        [JsonPropertyName("1008")] public string? BuyerContact { get; init; }
        [JsonPropertyName("1012")] public string? DateTime { get; init; }
        [JsonPropertyName("1020")] public ulong? Total { get; init; }
        [JsonPropertyName("1031")] public ulong? Cash { get; init; }
        [JsonPropertyName("1037")] public string? KktReg { get; init; }
        [JsonPropertyName("1038")] public uint? Session { get; init; }
        [JsonPropertyName("1040")] public uint? Doc { get; init; }
        [JsonPropertyName("1041")] public string? Drive { get; init; }
        [JsonPropertyName("1042")] public uint? DocSession { get; init; }
        [JsonPropertyName("1054")] public int? Type { get; init; }
        [JsonPropertyName("1055")] public int? Taxation { get; init; }
        [JsonPropertyName("1059")] public PositionDto[]? Items { get; init; }
        [JsonPropertyName("1077")] public string? Sign { get; init; }
        [JsonPropertyName("1081")] public ulong? NonCash { get; init; }
        [JsonPropertyName("1215")] public ulong? Advance { get; init; }
        [JsonPropertyName("1216")] public ulong? Credit { get; init; }
        [JsonPropertyName("1217")] public ulong? Provision { get; init; }
    }

    private readonly struct PositionDto
    {
        [JsonPropertyName("1023")] public decimal? Quantity { get; init; }
        [JsonPropertyName("1030")] public string? Name { get; init; }
        [JsonPropertyName("1043")] public ulong? Cost { get; init; }
        [JsonPropertyName("1079")] public ulong? Price { get; init; }
        [JsonPropertyName("1199")] public int? Tax { get; init; }
        [JsonPropertyName("1212")] public int? Type { get; init; }
        [JsonPropertyName("1214")] public int? CalcMethod { get; init; }
        [JsonPropertyName("2108")] public int? Unit { get; init; }
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
            Items = dto.Items is null ? null : [.. dto.Items.Select(ToPosition)],
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
            BuyerContact = dto.BuyerContact
        };
    }
}
