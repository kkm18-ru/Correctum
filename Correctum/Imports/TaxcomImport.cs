using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.Imports;

[ImportInfo("Такском-Касса")]
[TextBox("Логин", nameof(Login))]
[SecretBox("Пароль", nameof(Password))]
[TextBox("Номер договора", nameof(Agreement))]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("ККТ", nameof(Devices), nameof(SelectedDevices))]
[AuthorizationProperties(nameof(Login), nameof(Password), nameof(Agreement))]
public sealed class TaxcomImport : OfdImport<TaxcomClient>
{
    public string Login { get; set => SetField(ref field, value); } = string.Empty;
    public string Password { get; set => SetField(ref field, value); } = string.Empty;
    public string Agreement { get; set => SetField(ref field, value); } = string.Empty;    
    public DateRange DateRange { get; } = new();
    public ObservableCollection<string> Devices { get; } = [];
    public ObservableCollection<string> SelectedDevices { get; } = [];

    public override async IAsyncEnumerable<Receipt> ImportAsync
        ([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var ranges = new List<(string drive, uint min, uint max)>(SelectedDevices.Count);

        await FillRanges(ranges, cancellationToken);

        foreach (var (drive, min, max) in ranges)
        {
            for (var fd = min; fd <= max; fd++)
            {
                if (await client.FetchReceiptAsync(drive, fd, cancellationToken) is { } receipt)
                    yield return receipt;
            }
        }
    }

    private async Task FillRanges(List<(string, uint, uint)> ranges, CancellationToken cancellationToken)
    {
        foreach (var device in SelectedDevices)
        {
            await foreach (var drive in client.FetchDriveIdsAsync(device, cancellationToken))
            {
                var sessionIds = await client.FetchSessionIdsByPeriodAsync
                    (drive, DateRange.BeginDate, DateRange.EndDate, cancellationToken);
                if (sessionIds.Count == 0) continue;
                sessionIds.Sort();
                var docIds = await client.FetchDocumentNumbersBySessionAsync(drive, sessionIds[0], cancellationToken);
                var minDocId = docIds[0];
                if (sessionIds.Count > 1)
                    docIds = await client.FetchDocumentNumbersBySessionAsync(drive, sessionIds[^1], cancellationToken);
                var maxDocId = docIds[^1];
                ranges.Add((drive, minDocId, maxDocId));
            }
        }
    }

    protected override async Task InternalAuthAsync()
    {
        SelectedDevices.Clear();
        Devices.Clear();

        await client.AuthAsync(Login, Password, Agreement);

        await foreach (var outlet in client.FetchOutletIdsAsync())
            await foreach (var device in client.FetchDevicesAsync(outlet))
                Devices.Add(device);
    }
}
