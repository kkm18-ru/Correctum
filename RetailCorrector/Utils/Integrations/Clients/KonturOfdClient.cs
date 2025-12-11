using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Web;

namespace RetailCorrector.Utils.Integrations.Clients;

/// <summary>Обертка API-клиента Контур ОФД.</summary>
internal class KonturOfdClient : APIClient
{
    private const string Host = "https://ofd-api.kontur.ru";
    private readonly HttpClient _client;

    /// <summary>Инициализация <see href="KonturOfdClient"/></summary>
    public KonturOfdClient()
    {
        _client = new HttpClient(Handler, false) { BaseAddress = new(Host) };
        _client.DefaultRequestHeaders.Add("X-Kontur-Ofd-ApiKey", Configuration.Instance.Importes.KonturIntegratorKey);
    }

    /// <summary>Аутентификация в API Контур ОФД.</summary>
    /// <param name="login">Электронная почта.</param>
    /// <param name="passwd">Пользовательский пароль.</param>
    /// <param name="cancel">Токен, позволяющий отменить аутентификацию.</param>
    public async Task AuthAsync(string login, string passwd, CancellationToken cancel = default)
    {
        _client.DefaultRequestHeaders.Remove("Authorization");
        var encodedLogin = HttpUtility.UrlEncode(login);
        var url = $"https://api.kontur.ru/auth/authenticate-by-pass?login={encodedLogin}";
        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new StringContent(passwd, Encoding.UTF8);
        using var resp = await _client.SendAsync(req, cancel).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw new Exception(resp.StatusCode.ToString());
        using var stream= await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        var obj = await JsonSerializer.DeserializeAsync<AuthResponse>
            (stream, cancellationToken: cancel).ConfigureAwait(false);
        _client.DefaultRequestHeaders.Add("Authorization", $"auth.sid {obj.SessionId}");
    }

    /// <summary>Получение списка доступных организаций.</summary>
    /// <param name="cancel">Токен, позволяющий отменить перечисление организаций.</param>
    /// <returns>Список доступных организаций.</returns>
    public async IAsyncEnumerable<KonturOrganization> EnumerateOrganizationsAsync
        ([EnumeratorCancellation] CancellationToken cancel = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/v2/organizations");
        using var resp = await _client.SendAsync(req, cancel).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw new Exception(resp.StatusCode.ToString());
        using var stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        var enumerable = JsonSerializer.DeserializeAsyncEnumerable<KonturOrganization>
            (stream, cancellationToken: cancel).ConfigureAwait(false);
        await foreach (var item in enumerable) yield return item;
    }

    /// <summary>Получение списка регистрационных номеров ККТ организации.</summary>
    /// <param name="orgId">Идентификатор организации.</param>
    /// <param name="cancel">Токен, позволяющий отменить перечисление ККТ.</param>
    /// <returns>списка регистрационных номеров ККТ.</returns>
    public async IAsyncEnumerable<string> EnumerateDevicesAsync (string orgId, 
        [EnumeratorCancellation] CancellationToken cancel = default)
    {
        var url = $"/v2/organizations/{orgId}/cashboxes";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var resp = await _client.SendAsync(req, cancel).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw new Exception(resp.StatusCode.ToString());
        using var stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        var enumerable = JsonSerializer.DeserializeAsyncEnumerable<JsonObject>
            (stream, cancellationToken: cancel).ConfigureAwait(false);
        await foreach (var item in enumerable) 
            yield return item?["regNumber"]?.GetValue<string>() ?? "";
    }

    ~KonturOfdClient()
    {
        _client.Dispose();
    }
}

file struct AuthResponse
{
    [JsonPropertyName("Sid")]
    public string SessionId { get; set; }
}

public struct KonturOrganization
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("shortName")]
    public string Name { get; set; }

    public readonly override string ToString() => Name;
}