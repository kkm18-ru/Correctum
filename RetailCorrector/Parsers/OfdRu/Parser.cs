using RetailCorrector.Data;
using RetailCorrector.Utils.Integrations.Clients;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu: ParserBase<OfdRu, OfdRu.View>
    {
        public override string Name => "OFD.ru";

        public override ParseInfo GenerateInfo() =>
            parseInfo = new ParseInfo(Name,
                ("Начало", BeginDate.ToShortDateString()), 
                ("Конец", EndDate.ToShortDateString()));

        public readonly OfdRuClient Client = new();

        public override async IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            progress.Report(-((int)(EndDate - BeginDate).TotalDays + 1) * SelectedDevices.Count);
            var i = 0;
            for (var day = BeginDate; day <= EndDate; day = day.AddDays(1))
            {
                foreach(var device in SelectedDevices)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var arr = await Client.FetchReceiptsAsync
                        (parseInfo.Id, device, day, day.AddSeconds(86399), cancellationToken);
                    foreach (var receipt in arr) yield return receipt;
                    progress.Report(++i);
                }
            }
        }
    }
}
