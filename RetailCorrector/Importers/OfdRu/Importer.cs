using RetailCorrector.Data;
using Correctum.Core.Models;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal partial class OfdRu: ImporterBase<OfdRu, OfdRu.View>
{
    public override string Name => "OFD.ru";

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession(Name,
            ("Начало", DateRange.BeginDate.ToShortDateString()), 
            ("Конец", DateRange.EndDate.ToShortDateString()));

    public readonly OfdRuClient Client = new();

    public override async IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.ReportMaximum((uint)(DateRange.Count * SelectedDevices.Count));
        foreach(var day in DateRange)
        {
            foreach(var device in SelectedDevices)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var arr = await Client.FetchReceiptsAsync(parseInfo.Id, device, day, cancellationToken);
                foreach (var receipt in arr) yield return receipt;
                progress.ReportIncrement();
            }
        }
    }
    public override void OnClosing()
    {
        Client.Dispose();
    }
}
