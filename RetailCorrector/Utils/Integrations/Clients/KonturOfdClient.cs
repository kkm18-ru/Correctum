using System.Net.Http;

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

    ~KonturOfdClient() { _client.Dispose(); }
}