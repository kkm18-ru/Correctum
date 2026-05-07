using Correctum.Abstractions;
using Correctum.Attrubutes;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.Imports;

[ImportInfo("SabyOFD")]
public sealed class SabyImport : OfdImport<SabyClient>
{
    public string Login { get; set => SetField(ref field, value); } = string.Empty;
    public string Password { get; set => SetField(ref field, value); } = string.Empty;
    public Inn Inn { get; set => SetField(ref field, value); }
    public DateRange DateRange { get; } = new();
    public ObservableCollection<string> Devices { get; set; } = [];
    public ObservableCollection<string> SelectedDevices { get; set; } = [];

    public override async IAsyncEnumerable<Receipt> ImportAsync
        ([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var drives = new Dictionary<string, SabyDriveResponse[]>();
        foreach (var device in SelectedDevices)
            drives.Add(device, await client.FetchDrivesAsync(Inn, device, DateRange.BeginDate.ToDateTime(new()), DateRange.EndDate.ToDateTime(new()), cancellationToken));

        foreach (var device in Devices)
        {
            foreach (var drive in drives[device])
            {
                await foreach (var receipt in ParseByStorage(device, drive, cancellationToken))
                    yield return receipt;
            }
        }
    }

    private async IAsyncEnumerable<Receipt> ParseByStorage
        (string regId, SabyDriveResponse drive, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var baseRequest = new SabyReceiptsRequest(Inn, regId, drive.Id);
        var (start, end) = GetAvailablePeriod(drive);
        for (var day = start; day <= end; day = day.AddDays(1))
        {
            var request = baseRequest with { DateFrom = day, DateTo = day.AddSeconds(86_399) };
            await foreach (var receipt in ParseByDay(request, cancellationToken))
                yield return receipt;
        }
    }

    private async IAsyncEnumerable<Receipt> ParseByDay
        (SabyReceiptsRequest args, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var offset = 0u;
        while (true)
        {
            var receipts = await client.FetchReceiptsAsync(args, offset, cancellationToken);
            if (receipts is null || receipts.Length == 0) break;
            offset = receipts[^1].DocNumber.Value;
            foreach (var receipt in receipts) yield return receipt;
        }
    }

    private (DateTime, DateTime) GetAvailablePeriod(SabyDriveResponse drive)
    {
        var end = drive.DateEnd ?? DateTime.Now;
        end = end > DateRange.EndDate.ToDateTime(new()) ? DateRange.EndDate.ToDateTime(new()) : end;
        var start = drive.DateStart > DateRange.BeginDate.ToDateTime(new()) ? drive.DateStart : DateRange.BeginDate.ToDateTime(new());
        return (start.Date, end.Date);
    }

    protected override async Task InternalAuthAsync()
    {
        SelectedDevices.Clear();
        Devices.Clear();

        await client.AuthAsync(Login, Password);

        await foreach (var device in client.EnumerateDevicesAsync(Inn))
            Devices.Add(device);
    }
}
