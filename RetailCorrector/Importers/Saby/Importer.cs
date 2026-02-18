using RetailCorrector.Data;
using RetailCorrector.Utils.Integrations.Clients;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal partial class Saby : ImporterBase<Saby, Saby.View>
{
    public override string Name => "SabyOFD";
    private readonly SabyClient _client = new();

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession(Name, ("ИНН", Inn), 
            ("Старт", FromDate.ToString("dd'.'MM'.'yyyy")), 
            ("Конец", ToDate.ToString("dd'.'MM'.'yyyy")));

    public override async IAsyncEnumerable<Receipt> Import
        (IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var drives = new Dictionary<string, SabyDriveResponse[]>();
        foreach (var device in SelectedDevices)
            drives.Add(device, await _client.FetchDrivesAsync(Inn, device, FromDate, ToDate, cancellationToken));

        progress.Report(-drives.SelectMany(d => d.Value).Sum(d =>
        {
            var (start, end) = GetAvailablePeriod(d);
            return (end.Date - start.Date).Days + 1;
        }));
        ProgressValue = 0;
        foreach(var device in Devices)
        {
            foreach (var drive in drives[device])
            {
                await foreach (var receipt in ParseByStorage(device, drive, progress, cancellationToken))
                    yield return receipt;
            }
        }
    }

    private async IAsyncEnumerable<Receipt> ParseByStorage
        (string regId, SabyDriveResponse drive, IProgress<long> progress,  [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var baseRequest = new SabyReceiptsRequest(parseInfo.Id, Inn, regId, drive.Id);
        var (start, end) = GetAvailablePeriod(drive);
        for(var day = start; day <= end; day = day.AddDays(1))
        {
            var request = baseRequest with { DateFrom = day, DateTo = day.AddSeconds(86_399) };
            await foreach (var receipt in ParseByDay(request, cancellationToken))
                yield return receipt;
            progress.Report(++ProgressValue);
        }
    }

    private async IAsyncEnumerable<Receipt> ParseByDay
        (SabyReceiptsRequest args, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var offset = 0u;
        while (true)
        {
            var receipts = await _client.FetchReceiptsAsync(args, offset, cancellationToken);
            if (receipts is null || receipts.Length == 0) break;
            offset = receipts[^1].DocNumber;
            foreach (var receipt in receipts) yield return receipt;
        }
    }

    private (DateTime, DateTime) GetAvailablePeriod(SabyDriveResponse drive)
    {
        var end = drive.DateEnd ?? DateTime.Now;
        end = end > ToDate ? ToDate : end;
        var start = drive.DateStart > FromDate ? drive.DateStart : FromDate;
        return (start.Date, end.Date);
    }

    
    public override void OnClosing()
    {
        _client.Dispose();
    }
}