using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using System.Runtime.CompilerServices;

namespace Correctum.Importers;

internal partial class OfdRu: ImporterBase<OfdRu, OfdRu.View>
{
    public override string Name => "OFD.ru";

    public readonly OfdRuClient Client = new();

    public override async IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.ReportMaximum((uint)(DateRange.Count * SelectedDevices.Count));
        foreach(var day in DateRange)
        {
            foreach(var device in SelectedDevices)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var arr = await Client.FetchReceiptsAsync(device, day, cancellationToken);
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
