using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

internal readonly struct FirstOfdAuthRequest(string login, string password)
{
    [JsonPropertyName("login")] public string Login { get; } = login;
    [JsonPropertyName("password")] public string Password { get; } = password;
}
