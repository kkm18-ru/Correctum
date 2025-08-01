using RetailCorrector.Data;
using System.Net.Http;
using System.Text.Json.Nodes;

namespace RetailCorrector.Parsers.Taxcom
{
    [RegisterIntegration("Taxcom")]
    internal partial class TaxcomPresenter : ParserPresenter<TaxcomPresenter, TaxcomView>
    {
        public override async IAsyncEnumerable<Receipt> Parse(CancellationToken cancellationToken)
        {
            using var http = new HttpClient() 
            { 
                BaseAddress = new Uri("https://lk-ofd.taxcom.ru") 
            };
            await Init(http);
            await Auth(http);
            var cash = (await GetDevices(http))[DeviceId];
            bool hasNextPage = true;
            for (var i = 1; hasNextPage; i++)
            {
                JsonArray arr;
                (hasNextPage, arr) = await GetReceipts(http, cash, i);
                foreach (var doc in ParseReceipts(arr))
                    yield return await GetReceipt(http, doc);
            }
        }
    }
}
