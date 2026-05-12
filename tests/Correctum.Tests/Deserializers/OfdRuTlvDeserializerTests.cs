using Correctum.Core.Models;
using Correctum.Deserializers;
using Correctum.Utils.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Correctum.Tests.Deserializers;

public sealed class OfdRuTlvDeserializerTests
    : BaseDeserializerTests<OfdRuTlvDeserializer>
{
    protected override string GenerateText(Receipt receipt) =>
        JsonSerializer.Serialize(new ReceiptDto(receipt));

    private readonly struct ReceiptDto(Receipt doc)
    {
        [JsonPropertyName("1008")] public string BuyerContact { get; } = doc.BuyerContact!;
        [JsonPropertyName("1012")] public string DateTime { get; } = doc.Created!.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
        [JsonPropertyName("1020")] public ulong Total { get; } = (ulong)doc.TotalSum!.Value;
        [JsonPropertyName("1031")] public ulong Cash { get; } = (ulong)doc.Payment!.Value.CashSum!.Value;
        [JsonPropertyName("1037")] public string KktReg { get; } = doc.RegId!;
        [JsonPropertyName("1038")] public uint Session { get; } = doc.SessionId!.Value;
        [JsonPropertyName("1040")] public uint Doc { get; } = doc.DocNumber!.Value;
        [JsonPropertyName("1041")] public string Drive { get; } = doc.StorageId!;
        [JsonPropertyName("1042")] public uint DocSession { get; } = doc.DocInSession!.Value;
        [JsonPropertyName("1054")] public int Type { get; } = (int)doc.Type!.Value;
        [JsonPropertyName("1055")] public int Taxation { get; } = (int)doc.TaxationSystem!.Value;
        [JsonPropertyName("1059")] public PositionDto[] Items { get; } = [.. doc.Items!.Select(p => new PositionDto(p))];
        [JsonPropertyName("1077")] public string Sign { get; } = FiscalHelper.ToStringSign(uint.Parse(doc.DocSign!.Value.ToString()));
        [JsonPropertyName("1081")] public ulong NonCash { get; } = (ulong)doc.Payment!.Value.NonCashSum!.Value;
        [JsonPropertyName("1215")] public ulong Advance { get; } = (ulong)doc.Payment!.Value.AdvanceSum!.Value;
        [JsonPropertyName("1216")] public ulong Credit { get; } = (ulong)doc.Payment!.Value.CreditSum!.Value;
        [JsonPropertyName("1217")] public ulong Provision { get; } = (ulong)doc.Payment!.Value.ProvisionSum!.Value;
    }

    private readonly struct PositionDto(Position pos)
    {
        [JsonPropertyName("1023")] public decimal Quantity { get; } = pos.Quantity!.Value;
        [JsonPropertyName("1030")] public string Name { get; } = pos.Name!;
        [JsonPropertyName("1043")] public ulong Cost { get; } = (ulong)pos.Cost!.Value;
        [JsonPropertyName("1079")] public ulong Price { get; } = (ulong)pos.Price!.Value;
        [JsonPropertyName("1199")] public int Tax { get; } = pos.TaxRate!.Value.ToFns();
        [JsonPropertyName("1212")] public int Type { get; } = (int)pos.Type!;
        [JsonPropertyName("1214")] public int CalcMethod { get; } = (int)pos.CalcMethod!;
        [JsonPropertyName("2108")] public int Unit { get; } = (int)pos.Unit!;
    }
}
