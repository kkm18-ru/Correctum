using RetailCorrector.Attributes;
using RetailCorrector.Data;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Windows;

namespace RetailCorrector.Parsers
{
    [RegisterParser("OFD.ru (Петер-Сервис)")]
    internal partial class OfdRu: ParserBase<OfdRu, OfdRu.View>
    {
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
            var leftTries = Configuration.OfdRu.CountTries;
            do
            {
                leftTries--;
                await Task.Delay(Configuration.OfdRu.RequestDelay, cancellationToken);
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
    }
}
