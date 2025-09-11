using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RetailCorrector.Parsers;

internal partial class Saby
{
    public async Task Auth(HttpClient http)
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
        var json = JsonSerializer.Deserialize<dynamic>(respBody)!;
        http.DefaultRequestHeaders.Add("Cookie", $"sid={json.sid}");
    }
}