using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.ValueObjects;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using Correctum.ViewModels.Controls;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.Imports;

[ImportInfo("SabyOFD")]
[TextBox("Логин", nameof(Login))]
[TextBox("Пароль", nameof(Password))]
[TextBox("ИНН организации", nameof(Inn), "innConverter")]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("ККТ", nameof(SelectedDevices), nameof(Devices))]
[AuthorizationProperties(nameof(Login), nameof(Password), nameof(Inn))]
public sealed class SabyImport : OfdImport<SabyClient>
{
    public string Login { get; set => SetField(ref field, value); } = string.Empty;
    public string Password { get; set => SetField(ref field, value); } = string.Empty;
    public Inn Inn { get; set => SetField(ref field, value); }
    public DateRange DateRange { get; } = new();
    public ObservableCollection<string> Devices { get; set; } = [];
    public ObservableCollection<string> SelectedDevices { get; set; } = [];

    public override async IAsyncEnumerable<Receipt> ImportAsync
        (MultiProgress progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var drives = await GetDrivesAsync(progress, cancellationToken);

        foreach (var device in progress.Stage(Devices, "ККТ: {0}"))
        {
            foreach (var drive in progress.Stage(drives[device], "ФН: {0}"))
            {
                var baseRequest = new SabyReceiptsRequest(Inn, device, drive.Id);
                var beginDate = DateTime.Max(drive.DateStart.Date, DateRange.BeginDateTime.Date);
                var endDate = !drive.DateEnd.HasValue ? DateRange.EndDateTime.Date : 
                        DateTime.Min(drive.DateEnd!.Value.Date, DateRange.EndDateTime.Date);
                foreach(var date in progress.Stage(new DateRange(beginDate, endDate), "Дата: {0:dd'.'MM'.'yyyy}"))
                {
                    var request = baseRequest with
                    {
                        DateFrom = date.ToDateTime(TimeOnly.MinValue),
                        DateTo = date.ToDateTime(TimeOnly.MaxValue)
                    };
                    var offset = 0u;
                    while (true)
                    {
                        var receipts = await client.FetchReceiptsAsync(request, offset, cancellationToken);
                        if (receipts is null || receipts.Length == 0) break;
                        offset = receipts[^1].DocNumber.Value;
                        foreach (var receipt in receipts) yield return receipt;
                    }
                }
            }
        }
    }

    private async Task<Dictionary<string, SabyDriveResponse[]>> GetDrivesAsync
        (MultiProgress progress, CancellationToken cancellationToken)
    {
        using var _ = progress.Busy("Получение списка ФН...");
        var result = new Dictionary<string, SabyDriveResponse[]>();
        foreach (var device in SelectedDevices)
            result.Add(device, await client.FetchDrivesAsync(Inn, device, DateRange.BeginDate.ToDateTime(new()), DateRange.EndDate.ToDateTime(new()), cancellationToken));
        return result;
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
