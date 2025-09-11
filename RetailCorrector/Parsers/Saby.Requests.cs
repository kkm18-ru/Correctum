using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RetailCorrector.Parsers;

internal partial class Saby
{
    private async Task Auth(HttpClient http)
    {
        var content = new
        {
            app_client_id = AppClientId,
            login = Login,
            password = Password,
        };
        using var req = new HttpRequestMessage(HttpMethod.Post, "/oauth/service/");
        req.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
        using var resp = await http.SendAsync(req);
        var respBody = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode) throw new ArgumentException(respBody.Split(": ")[^1].Split(')')[0]);
        var json = JsonNode.Parse(respBody)!;
        http.DefaultRequestHeaders.Add("Cookie", $"sid={json["sid"]!.GetValue<string>()}");
    }

    private async Task<(IEnumerable<JsonNode?>, bool)> ParsePage(HttpClient http, DateTime day, int? offset)
    {
        var txtDay = day.ToString("yyyy'-'MM'-'dd");
        var urlBuilder = new StringBuilder();
        urlBuilder.Append($"/ofd/v1/orgs/{INN}");
        urlBuilder.Append($"/kkts/{Device}");
        urlBuilder.Append($"/storages/{Storage}/docs");
        urlBuilder.Append($"?dateFrom={txtDay}T00:00:00");
        urlBuilder.Append($"&dateTo={txtDay}T23:59:59");
        if(offset is not null) urlBuilder.Append($"&startId={offset}");
        urlBuilder.Append($"&limit={Configuration.Saby.CountDocsByRequest}");
        using var resp = await http.GetAsync(urlBuilder.ToString());
        var content = await resp.Content.ReadAsStringAsync();
        var arr = JsonNode.Parse(content)!.AsArray()!;
        return (arr.Where(node => node is not null && (node["receipt"] is not null || node["receiptCorrection"] is not null))
            .Select(i => i!["receipt"] ?? i["receiptCorrection"]), arr.Count == Configuration.Saby.CountDocsByRequest);
    }
}