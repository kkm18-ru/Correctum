using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RetailCorrector.Utils.Integrations.Clients;

/// <summary>Обертка API-клиента Контур ОФД.</summary>
internal class KonturOfdClient : APIClient
{
    private const string Host = "https://ofd.kontur.ru/api/v1/";
    private readonly HttpClient _client = new(Handler, false) { BaseAddress = new(Host) };

    /// <summary>Установка аутентификационного токена</summary>
    /// <param name="token">Аутентификационный токен</param>
    public void SetAuthToken(string token) =>
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

    /// <summary>Перечисление доступных организаций</summary>
    /// <param name="cancellationToken">Токен, позволяющий отменить действие</param>
    /// <returns>Асинхронное перечисление организации</returns>
    public async IAsyncEnumerable<KonturOrganization> EnumerateOrganizations
        ([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/organizations");
        using var resp = await _client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var enumerable = JsonSerializer.DeserializeAsyncEnumerable<KonturOrganization>(stream, cancellationToken: cancellationToken);
        await foreach (var item in enumerable) yield return item;
    }

    /// <summary>Перечисление доступных ККТ организации</summary>
    /// <param name="org">Организация</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить действие</param>
    /// <returns>Асинхронное перечисление регистрационных номеров ККТ</returns>
    public async IAsyncEnumerable<string> EnumerateDevices
        (KonturOrganization org, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"/organizations/{org.Id:D}/cashboxes");
        using var resp = await _client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var enumerable = JsonSerializer.DeserializeAsyncEnumerable<Device>(stream, cancellationToken: cancellationToken);
        await foreach (var item in enumerable) yield return item.RegId;
    }

    ~KonturOfdClient() { _client.Dispose(); }
}

public struct KonturOrganization
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("inn")]
    public string Inn { get; set; }

    [JsonPropertyName("shortName")]
    public string Name { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; }

    [JsonPropertyName("isBlocked")]
    public bool Blocked { get; set; }

    [JsonPropertyName("creationDate")]
    public DateTimeOffset Created { get; set; }
}

file struct Device
{
    [JsonPropertyName("regNumber")]
    public string RegId { get; set; }
}