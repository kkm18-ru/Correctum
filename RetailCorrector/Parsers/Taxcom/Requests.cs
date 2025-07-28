using System.Net.Http;
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
    }
}
