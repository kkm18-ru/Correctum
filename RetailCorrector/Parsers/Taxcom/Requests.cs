using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RetailCorrector.Parsers.Taxcom
{
    internal partial class TaxcomPresenter
    {
        private static async Task Init(HttpClient http)
        {
            using var resp = await http.GetAsync("/api/account/login");
            var content = await resp.Content.ReadAsStringAsync();
            var data = JsonNode.Parse(content)!;
            http.DefaultRequestHeaders.Add("csrf", data["csrf"]!.GetValue<string>());
        }

        private async Task Auth(HttpClient http)
        {
            var data = new
            {
                login = Login,
                password = Password,
                externalSystem = (int?)null
            };
            var dataJson = JsonSerializer.Serialize(data);
            var content = new StringContent(dataJson, Encoding.UTF8, "application/json");
            using var resp = await http.PostAsync("/api/account/login", content);
            if (!resp.Headers.TryGetValues("Set-Cookie", out var cookies))
                throw new HttpRequestException();
            cookies = cookies.Select(cookie => cookie.Split("; ")[0]);
            http.DefaultRequestHeaders.Add("Cookie", cookies);
        }
    }
}
