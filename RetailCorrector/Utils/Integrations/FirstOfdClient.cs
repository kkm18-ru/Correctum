using System.Net.Http;

namespace RetailCorrector.Utils.Integrations;

internal class FirstOfdClient : IDisposable
{
    private readonly HttpClient client;

    public FirstOfdClient(string apiKey)
    {
        var uri = new Uri("https://universal-api.1-ofd.ru/api/rent/");
        client = new HttpClient { BaseAddress = uri };
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
    }

    public void Dispose() => client.Dispose();
}
