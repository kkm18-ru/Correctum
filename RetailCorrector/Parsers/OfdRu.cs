using RetailCorrector.Data;
using RetailCorrector.Utils;
using Serilog;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Windows;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu: ParserBase<OfdRu, OfdRu.View>
    {
        public override string Name => "OFD.ru";

        public override ParseInfo GenerateInfo() =>
            parseInfo = new ParseInfo("OFD.ru", 
                ("ИНН", Vatin), 
                ("РНМ", DeviceId),
                ("Начало", BeginDate.ToShortDateString()), 
                ("Конец", EndDate.ToShortDateString()));

        private string Url(DateTime date)
        {
            var dayText = date.ToString("yyyy'-'MM'-'dd");
            return $"https://ofd.ru/api/integration/v2/inn/{Vatin}/kkt/{DeviceId}/receipts-info?" +
                $"AuthToken={Token}&dateFrom={dayText}T00:00:00&dateTo={dayText}T23:59:59";
        }

        public override async IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var http = new HttpClient();
            progress.Report(-(int)(EndDate - BeginDate).TotalDays - 1);
            for (var day = BeginDate; day <= EndDate; day = day.AddDays(1))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var array = await GetContent(http, day, cancellationToken);
                progress.Report((long)(day - BeginDate).TotalDays + 1);
                if (array is null)
                {
                    MessageBox.Show($"Не удалось получить чеки за {day:d}");
                    continue;
                }
                foreach (var item in array)
                    yield return ParseReceipt(item!, parseInfo.Id);
            }
        }

        private async Task<JsonArray?> GetContent(HttpClient http, DateTime day, CancellationToken cancellationToken)
        {
            var leftTries = Configuration.Instance.HttpTriesCount;
            do
            {
                leftTries--;
                await Task.Delay(Configuration.Instance.HttpRequestDelay, cancellationToken);
                using var resp = await http.GetAsync(Url(day), cancellationToken);
                if (!resp.IsSuccessStatusCode) continue;
                var content = await resp.Content.ReadAsStringAsync(cancellationToken);
                var json = JsonNode.Parse(content);
                if (json is null) continue;
                if (json["Data"] is null) continue;
                return json["Data"]!.AsArray();
            } while (leftTries > 0);
            return null;
        }

        private IEnumerable<string> GetDevices()
        {
            var code = -1;
            var content = "";
            try
            {
                using var http = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Get, $"https://ofd.ru/api/integration/v2/kkts?AuthToken={Token}");
                using var resp = http.Send(req);
                code = (int)resp.StatusCode;
                content = resp.Content.ReadAsStringAsync().Result;
                var json = JsonNode.Parse(content)!;
                return json["Data"]!.AsArray().Select(d => d!["KktRegId"]!.GetValue<string>());
            }
            catch(Exception e)
            {
                MessageBox.Show("Не удалось получить информацию об ККТ организации");
                Log.Error(e, "Не удалось получить информацию об ККТ организации ({Code}: {Content})", code, content);
                return [];
            }
        }
    }
}
