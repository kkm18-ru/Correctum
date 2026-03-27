using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient.Models;

internal readonly struct FirstOfdKktSearchRequest()
{
    [JsonPropertyName("criteria")] public string Criteria { get; } = string.Empty;
    [JsonPropertyName("monitoringFilter")] public string Filter { get; } = string.Empty;
}
