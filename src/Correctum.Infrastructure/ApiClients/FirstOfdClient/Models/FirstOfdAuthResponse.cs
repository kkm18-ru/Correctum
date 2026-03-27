using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient.Models;

[method: JsonConstructor]
internal readonly struct FirstOfdAuthResponse(string authToken)
{
    public string Token { get; } = authToken;
}
