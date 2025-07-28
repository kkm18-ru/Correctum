using RetailCorrector.Data;
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

        private async Task<Dictionary<string, int>> GetDevices(HttpClient http)
        {
            var dict = new Dictionary<string, int>();
            var resp = await http.GetStringAsync("/api/cashdesk/kkt");
            var json = JsonNode.Parse(resp)!["data"]?["collection"]?.AsArray();
            foreach(var kass in json)
            {
                if (kass is null) continue;
                var info = kass["cashdeskInfo"];
                if (info is null) continue;
                if (info["status"]!.GetValue<string>() != "0")
                    continue;
                var regId = info["registrationNumber"]!.GetValue<string>();
                var id = info["cashdeskId"]!.GetValue<int>();
                dict.Add(regId, id);
            }
            return dict;
        }

        private async Task<(bool, JsonArray)> GetReceipts(HttpClient http, int devId, int pageId = 1)
        {
            var builder = new StringBuilder($"/api/cashdesk/documents/{devId}");
            builder.Append($"?paging[count]=100");
            builder.Append($"&paging[page]={pageId}");
            builder.Append($"&filters[toDate]={BeginDate:yyyy'-'MM'-'dd}");
            builder.Append($"&filters[fromDate]={EndDate:yyyy'-'MM'-'dd}");
            builder.Append($"&filters[documentTypeID]=3");
            var resp = await http.GetStringAsync(builder.ToString());
            var json = JsonNode.Parse(resp)["data"];
            return (json["paging"]["pages"].GetValue<int>() > pageId, json["collection"].AsArray());
        }

        private async Task<Guid> GetReceiptId(HttpClient http, (string, int) docInfo)
        {
            var uri = $"/api/cashdesk/documents/print?kktId={DeviceId}&fp={docInfo.Item1}&documentNumber={docInfo.Item2}";
            var resp = await http.GetStringAsync(uri);
            var json = JsonNode.Parse(resp);
            return new Guid(json["data"]["receiptId"].GetValue<string>());
        }

        private async Task<Receipt> GetReceipt(HttpClient http, (string, int) docInfo)
        {
            var id = await GetReceiptId(http, docInfo);
            var uri = $"/api/cashdesk/documents/getTaggedReceipt?receiptGuid={id:D}";
            var resp = await http.GetStringAsync(uri);
            var json = JsonNode.Parse(resp)["data"]["Document"]["Value"].AsArray();
            return ParseReceipt(json);
        }
    }
}
