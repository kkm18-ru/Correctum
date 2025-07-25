using RetailCorrector.Data;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Windows;

namespace RetailCorrector.Parsers.OfdRu
{
    internal partial class OfdRuPresenter : IParserPresenter<OfdRuPresenter, OfdRuView>
    {
        public static string DisplayName { get; } = "OFD.ru (Петер-Сервис)";
        public static OfdRuView ShowView() => new();

        private string Url(DateTime date)
        {
            var dayText = date.ToString("yyyy'-'MM'-'dd");
            return $"https://ofd.ru/api/integration/v2/inn/{Vatin}/kkt/{DeviceId}/receipts-info?" +
                $"AuthToken={Token}&dateFrom={dayText}T00:00:00&dateTo={dayText}T23:59:59";
        }

        public async IAsyncEnumerable<Receipt> Parse([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var http = new HttpClient();
            var baseUrl = $"";
            CountDays = (int)(EndDate - BeginDate).TotalDays + 1;
            for (var day = BeginDate; day <= EndDate; day = day.AddDays(1))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var array = await GetContent(http, day, cancellationToken);
                CurrentDayNumber++;
                if(array is null)
                {
                    MessageBox.Show($"Не удалось получить чеки за {day:d}");
                    continue;
                }
                foreach (var item in array)
                    yield return ParseReceipt(item!);
            }
        }

        private async Task<JsonArray?> GetContent(HttpClient http, DateTime day, CancellationToken cancellationToken)
        {
            var leftTries = COUNT_TRIES;
            do
            {
                leftTries--;
                await Task.Delay(REQUEST_DELAY, cancellationToken);
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
