using RetailCorrector.Data;
using RetailCorrector.Utils.Integrations.Clients;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Parsers;

internal partial class Saby : ParserBase<Saby, Saby.View>
{
    public override string Name => "SabyOFD";
    private readonly SabyClient _client = new();

    public override ParseInfo GenerateInfo() =>
        parseInfo = new ParseInfo(Name, ("ИНН", Inn), ("ККТ", Device), 
            ("Старт", FromDate.ToString("dd'.'MM'.'yyyy")), 
            ("Конец", ToDate.ToString("dd'.'MM'.'yyyy")));

    public override async IAsyncEnumerable<Receipt> Parse
        (IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var drives = await _client.FetchDrivesAsync(Inn, Device, FromDate, ToDate, cancellationToken);
        progress.Report(-drives.Sum(drive =>
        {
            var (start, end) = GetAvailablePeriod(drive);
            return (end.Date - start.Date).Days + 1;
        }));
        ProgressValue = 0;
        foreach (var drive in drives)
        {
            await foreach (var receipt in ParseByStorage(drive, progress, cancellationToken))
                yield return receipt;
        }
    }

    private async IAsyncEnumerable<Receipt> ParseByStorage
        (SabyDriveResponse drive,IProgress<long> progress,  [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var baseRequest = new SabyReceiptsRequest(parseInfo.Id, Inn, Device, drive.Id);
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
        var offset = 0;
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
}