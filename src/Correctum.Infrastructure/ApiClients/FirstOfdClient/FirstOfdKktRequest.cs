using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

internal readonly struct FirstOfdKktRequest()
{
    [JsonPropertyName("criteria")] public string Criteria { get; } = string.Empty;
    [JsonPropertyName("monitoringFilter")] public string Filter { get; } = string.Empty;
}
