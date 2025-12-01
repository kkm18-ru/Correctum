using System.Net.Http;
using System.Text.Json.Nodes;
using System.Windows;
using Organisation = (string id, string name);
using KKT = (int id, string regNumb, string[] drives);

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

    /// <summary>
    /// Получение списка ККТ организации
    /// </summary>
    /// <param name="orgKey">Идентификатор организации</param>
    /// <param name="cancel">Токен, отменяющий действие</param>
    /// <returns>Массив информации о ККТ</returns>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="UriFormatException"/>
    /// <exception cref="OperationCanceledException"/>
    public async Task<KKT[]> FetchDevicesAsync(string orgKey, CancellationToken cancel = default)
    {
        using var resp = await client.GetAsync($"v2/organisations/{orgKey}/kkms", cancel);
        var respStream = await resp.Content.ReadAsStreamAsync(cancel);
        var json = await JsonNode.ParseAsync(respStream, cancellationToken: cancel);
        var kkts = json!["kkms"]!.AsArray();
        var result = new KKT[kkts.Count];
        for (var i = 0; i < result.Length; i++)
        {
            var item = kkts[i]!;
            var id = item["virtualKkmId"]!.GetValue<int>();
            var regNumb = item["kkmRegId"]!.GetValue<string>();
            var jsonDrives = item["fiscalFrives"]!.AsArray();
            var drives = new string[jsonDrives.Count];
            for (var j = 0; j < drives.Length; j++)
                drives[j] = jsonDrives[j]!["fsFactoryNumber"]!.GetValue<string>();
            result[i] = (id, regNumb, drives);
        }
        return result;
    }
}
