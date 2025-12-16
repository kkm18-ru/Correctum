using System.Net.Http;
using System.Net.Http.Json;
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

    /// <summary>Перечисление чеков за период по ККТ</summary>
    /// <param name="org">Организация</param>
    /// <param name="beginDate">Начальная дата периода.</param>
    /// <param name="endDate">Конечная дата периода.</param>
    /// <param name="regId">Регистрационный номер ККТ</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить действие</param>
    /// <returns>Асинхронное перечисление идентификатора документа</returns>
    public async IAsyncEnumerable<KonturReceiptId> GetReceiptsId
        (KonturOrganization org, DateTime beginDate, DateTime endDate, string regId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"/organizations/{org.Id:D}/cashreceiptbriefs/search");
        req.Content = JsonContent.Create(new ReceiptsRequest(beginDate, endDate, regId));
        using var resp = await _client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var obj = await JsonSerializer.DeserializeAsync<ReceiptsResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        foreach (var item in obj.Items) yield return item.Id;
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

file class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => 
        writer.WriteStringValue(value.ToString("yyyy-MM-ddT00:00:00.000'Z'"));
}

file readonly struct ReceiptsRequest(DateTime beginDate, DateTime endDate, string device)
{
    [JsonPropertyName("from"), JsonConverter(typeof(DateTimeConverter))]
    public DateTime BeginDate { get; } = beginDate;

    [JsonPropertyName("to"), JsonConverter(typeof(DateTimeConverter))]
    public DateTime EndDate { get; } = endDate; // API воспринимает это свойство некорректно

    [JsonPropertyName("count")]
    public uint Count { get; } = 1000;

    [JsonPropertyName("cashboxRegNumber")]
    public string RegId { get; } = device;
}

file struct ReceiptsResponse
{
    [JsonPropertyName("items")]
    public ReceiptsItem[] Items { get; set; }
}

file struct ReceiptsItem
{
    [JsonPropertyName("id")]
    public KonturReceiptId Id { get; set; }
}

public struct KonturReceiptId
{
    [JsonPropertyName("organizationId")]
    public Guid OrgId { get; set; }

    [JsonPropertyName("cashboxRegNumber")]
    public string RegId { get; set; }

    [JsonPropertyName("fnSerialNumber")]
    public string DriveId { get; set; }

    [JsonPropertyName("fiscalDocumentNumber")]
    public uint DocId { get; set; }

    public readonly override string ToString() =>
        $"/organizations/{OrgId:D}/cashboxes/{RegId}/fns/{DriveId}/documents/{DocId}";
}
