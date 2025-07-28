using System.Text.Json.Nodes;

namespace RetailCorrector.Parsers.Taxcom
{
    internal partial class TaxcomPresenter
    {
        private static IEnumerable<(string, int)> ParseReceipts(JsonArray arr)
        {
            foreach (var doc in arr)
                yield return (doc["fiscalSign"].GetValue<string>(),
                    doc["documentNumber"].GetValue<int>());
        }
    }
}
