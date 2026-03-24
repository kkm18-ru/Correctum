using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

[method: JsonConstructor]
internal readonly struct SabyDocument(SabyReceipt? receipt, SabyReceipt? receiptCorrection)
{
    public SabyReceipt? Receipt { get; } = receipt ?? receiptCorrection;
}