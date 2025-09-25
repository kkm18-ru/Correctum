using RetailCorrector.Attributes;
using RetailCorrector.Data;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Parsers;

[RegisterParser(DisplayName = "SabyOFD")]
internal partial class Saby : ParserBase<Saby, Saby.View>
{
    public override ParseInfo GenerateInfo() =>
        parseInfo = new("SabyOFD", ("ИНН", INN), ("ККТ", Device), ("ФН", Storage), 
            ("Старт", FromDate.ToString("dd'.'MM'.'yyyy")), 
            ("Конец", ToDate.ToString("dd'.'MM'.'yyyy")));

    public override async IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var http = new HttpClient
        {
            BaseAddress = new Uri(BaseHost)
        };
        await Auth(http);
        progress.Report(-(int)(ToDate - FromDate).TotalDays - 1);
        for(var day = FromDate; day <= ToDate; day = day.AddDays(1))
        {
            int? offset = null;
            while (true)
            {
                var (arr, @continue) = await ParsePage(http, day, offset);
                offset = arr.LastOrDefault()?["fiscalDocumentNumber"]!.GetValue<int>() ?? null;
                foreach (var node in arr)
                {
                    if (node is null) continue;
                    yield return ToReceipt(node);
                }
                if (!@continue) break;
            }
            progress.Report((long)(day - FromDate).TotalDays + 1);
        }
    }
}