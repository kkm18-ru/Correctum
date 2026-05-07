using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.Imports;

[ImportInfo("SabyOFD")]
[TextBox("Логин", nameof(Login))]
[SecretBox("Пароль", nameof(Password))]
[TextBox("ИНН организации", nameof(Inn))]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("ККТ", nameof(Devices), nameof(SelectedDevices))]
[AuthorizationProperties(nameof(Login), nameof(Password))]
public sealed class SabyImport : OfdImport<SabyClient>
{
    public string Login { get; set => SetField(ref field, value); } = string.Empty;
    public string Password { get; set => SetField(ref field, value); } = string.Empty;
    public string Inn { get; set => SetField(ref field, value); } = string.Empty;
    public DateRange DateRange { get; } = new();
    public ObservableCollection<string> Devices { get; set; } = [];
    public ObservableCollection<string> SelectedDevices { get; set; } = [];

    public override async IAsyncEnumerable<Receipt> ImportAsync
        ([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var drives = new Dictionary<string, SabyDriveResponse[]>();
        foreach (var device in SelectedDevices)
            drives.Add(device, await client.FetchDrivesAsync(Core.ValueObjects.Inn.Parse(Inn), device, DateRange.BeginDate.ToDateTime(new()), DateRange.EndDate.ToDateTime(new()), cancellationToken));

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
        var baseRequest = new SabyReceiptsRequest(Core.ValueObjects.Inn.Parse(Inn), regId, drive.Id);
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

        await foreach (var device in client.EnumerateDevicesAsync(Core.ValueObjects.Inn.Parse(Inn)))
            Devices.Add(device);
    }
}
