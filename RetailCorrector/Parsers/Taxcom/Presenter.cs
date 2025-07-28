using RetailCorrector.Data;
using System.Net.Http;

namespace RetailCorrector.Parsers.Taxcom
{
    internal partial class TaxcomPresenter : IParserPresenter<TaxcomPresenter, TaxcomView>
    {
        public static string DisplayName { get; } = "Taxcom";
        public static TaxcomView ShowView() => new();

        public async IAsyncEnumerable<Receipt> Parse(CancellationToken cancellationToken)
        {
            using var http = new HttpClient() 
            { 
                BaseAddress = new Uri("https://lk-ofd.taxcom.ru") 
            };
            await Init(http);
            await Auth(http);
        }
    }
}
