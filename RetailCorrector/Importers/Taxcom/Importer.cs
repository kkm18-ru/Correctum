using System.Runtime.CompilerServices;
using RetailCorrector.Data;
using RetailCorrector.Exceptions;
using RetailCorrector.Utils.Integrations.Clients;
using Range = (string drive, uint min, uint max);

namespace RetailCorrector.Importers;

internal partial class Taxcom : ImporterBase<Taxcom, Taxcom.View>
{
    public override string Name => "Такском-Касса";
    private readonly TaxcomClient _client = new();

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession(Name);

    public override async IAsyncEnumerable<Receipt> Import
        (IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var ranges = new List<Range>(SelectedDevices.Count);

        await FillRanges(progress, ranges, cancellationToken);

        var i = 0L;
        foreach (var range in ranges)
        {
            for (var j = range.min; j <= range.max; j++)
            {
                if (await _client.FetchReceiptAsync(range.drive, j, parseInfo.Id, cancellationToken) is { } receipt)
                    yield return receipt;
                progress.Report(++i);
            }
        }
    }

    private async Task FillRanges(IProgress<long> progress, List<Range> ranges, CancellationToken cancellationToken)
    {
        foreach (var device in SelectedDevices)
        {
            await foreach (var drive in _client.FetchDriveIdsAsync(device, cancellationToken))
            {
                var sessionIds = await _client.FetchSessionIdsByPeriodAsync
                    (drive, DateRange.BeginDate, DateRange.EndDate, cancellationToken);
                if(sessionIds.Count == 0) continue;
                sessionIds.Sort();
                var docIds = await _client.FetchDocumentNumbersBySessionAsync(drive, sessionIds[0], cancellationToken);
                var minDocId = docIds[0];
                if (sessionIds.Count > 1)
                    docIds = await _client.FetchDocumentNumbersBySessionAsync(drive, sessionIds[^1], cancellationToken);
                var maxDocId = docIds[^1];
                ranges.Add((drive, minDocId, maxDocId));
                progress.Report(-ranges.Sum(r => r.Item3 - r.Item2 + 1));
            }
        }
    }

    public override void OnClosing()
    {
        _client.Dispose();
    }
}