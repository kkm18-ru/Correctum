using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

[method: JsonConstructor]
public readonly struct SabyFiscalDrive(string storageId, DateTime effectiveFrom, DateTime? effectiveTo)
{
    public string SerialNumber { get; } = storageId;
    public DateTime ActivatedDate { get; } = effectiveFrom;
    public DateTime? ClosedDate { get; } = effectiveTo;
}
