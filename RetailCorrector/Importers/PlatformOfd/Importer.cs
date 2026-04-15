using RetailCorrector.Data;
using Correctum.Core.Models;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal sealed partial class PlatformOfd : ImporterBase<PlatformOfd, PlatformOfd.View>
{
    public readonly PlatformaOfdClient Client = new();

    public override string Name => "Платформа ОФД";

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession(Name);

    public override async IAsyncEnumerable<Receipt> Import
        (BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.ReportMaximum((uint)(DateRange.Count * SelectedDevices.Count));
        foreach(var date in DateRange)
        {
            foreach(var Device in SelectedDevices)
            {
                var references = await Client.FetchReceiptsAsync(date, Device, cancellationToken);
                foreach (var @ref in references)
                    if(await Client.FetchReceiptAsync(@ref, parseInfo.Id, cancellationToken) is {} receipt)
                        yield return receipt;
                progress.ReportIncrement();
            }
        }
    }
    public override void OnClosing()
    {
        Client.Dispose();
    }
}
