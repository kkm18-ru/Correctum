using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

[method: JsonConstructor]
internal readonly struct SabyKkt(string regId)
{
    public string RegNumber { get; } = regId;
}
