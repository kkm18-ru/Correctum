using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

[method: JsonConstructor]
internal readonly struct SabyAuthResponse(string sid)
{
    public string SessionId { get; } = sid;
}
