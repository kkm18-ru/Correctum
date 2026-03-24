using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

internal readonly struct SabyAuthRequest(string login, string password, bool asFranchise)
{
    [JsonPropertyName("login")] public string Login { get; } = login;
    [JsonPropertyName("password")] public string Password { get; } = password;
    [JsonPropertyName("app_client_id")] public string AppId { get; } = 
        asFranchise ? "2248309793600149" : "1025293145607151";
}
