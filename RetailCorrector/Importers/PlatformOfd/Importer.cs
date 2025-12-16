using System.Runtime.CompilerServices;
using RetailCorrector.Data;
using RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Importers;

internal sealed partial class PlatformOfd : ImporterBase<PlatformOfd, PlatformOfd.View>
{
    public readonly PlatformaOfdClient Client = new();

    public override string Name => "Платформа ОФД";

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession(Name);

    public override async IAsyncEnumerable<Receipt> Import
        (IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.Report(-((long)Math.Ceiling((EndDate - BeginDate).TotalDays / 7) * SelectedDevices.Count));
        for (var (d, i) = (BeginDate, 0); d < EndDate; d = d.AddDays(7), i++)
        {
            foreach(var Device in SelectedDevices)
            {
                var end = d.AddSeconds(604_799);
                end = end > EndDate ? EndDate : end;
                var references = await Client.FetchReceiptsAsync(d, end, Device, cancellationToken);
                foreach (var @ref in references)
                    yield return await Client.FetchReceiptAsync(@ref, parseInfo.Id, cancellationToken);
                progress.Report(i);
            }
        }
    }
}
