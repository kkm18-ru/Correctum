using System.Net.Http;

namespace RetailCorrector.Utils.Integrations.Clients;

/// <summary>Обертка API-клиента Контур ОФД.</summary>
internal class KonturOfdClient : APIClient
{
    private const string Host = "https://ofd-api.kontur.ru";
    private readonly HttpClient _client = new(Handler, false) { BaseAddress = new(Host) };

    ~KonturOfdClient()
    {
        _client.Dispose();
    }
}
