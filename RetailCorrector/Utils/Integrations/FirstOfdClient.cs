using System.Net.Http;
using System.Text.Json.Nodes;
using System.Windows;
using Organisation = (string id, string name);

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

    /// <summary>
    /// Получение списка организаций
    /// </summary>
    /// <param name="cancel">Токен, отменяющий действие</param>
    /// <returns>Массив идентификаторов и названий организаций</returns>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="UriFormatException"/>
    /// <exception cref="OperationCanceledException"/>
    public async Task<Organisation[]> FetchOrganisationsAsync(CancellationToken cancel = default)
    {
        using var resp = await client.GetAsync("v2/organisations", cancel);
        var respStream = await resp.Content.ReadAsStreamAsync(cancel);
        var json = await JsonNode.ParseAsync(respStream, cancellationToken: cancel);
        var array = json!.AsArray();
        var result = new Organisation[array.Count];
        for (var i = 0; i < result.Length; i++)
        {
            var item = array[i]!;
            var key = item["organisationKey"];
            var displayName = (item["organizationName"] ?? item["inn"]!).GetValue<string>();
            if (key is null)
            {
                MessageBox.Show($"Не удалось получить идентификатор \"{displayName}\"");
                continue;
            }
            result[i] = (key.GetValue<string>(), displayName);
        }
        return result;
    }
}
