using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RetailCorrector.Utils.Integrations.Clients;

public sealed class FirstOfdClient(string host = "https://org.1-ofd.ru") : ApiClient<FirstOfdClient>(host)
{
    public async Task AuthAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/cp-core/user/login")
        {
            Content = JsonContent.Create(new FirstOfdAuthRequest(login, password))
        };
        using var resp = await Client.SendAsync(req, cancellationToken);
        if (!resp.IsSuccessStatusCode)
        {
            await using var stream = await resp.Content.ReadAsStreamAsync(CancellationToken.None);
            var exc = await JsonSerializer.DeserializeAsync<FirstOfdException>(stream, cancellationToken: CancellationToken.None);
            throw exc ?? new HttpRequestException("Неизвестная ошибка сервера.", null, resp.StatusCode);
        }
    }
}

internal readonly record struct FirstOfdAuthRequest(string login, string password);

public class FirstOfdException : HttpRequestException
{
    [JsonPropertyName("message")] public string MessageId { get; set; }
    [JsonPropertyName("localizedMessage")] public string LocalizedMessage { get; set; }
    [JsonPropertyName("payload")] public object? Payload { get; set; }

    public override string ToString() => 
        string.IsNullOrWhiteSpace(LocalizedMessage) 
        ? MessageId 
        : LocalizedMessage;
}
