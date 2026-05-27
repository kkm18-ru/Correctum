using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using Correctum.ViewModels.Controls;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Correctum.Imports;

[ImportInfo("Такском-Касса")]
[TextBox("Логин", nameof(Login))]
[TextBox("Пароль", nameof(Password))]
[TextBox("Номер договора", nameof(Agreement))]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("ККТ", nameof(SelectedDevices), nameof(Devices))]
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
        (MultiProgress progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        #pragma warning disable S3973
        var drives = await GetDriveByKktAsync(progress, cancellationToken);
        foreach (var date in progress.Stage(DateRange, "Дата: {0}"))
        foreach (var kkt in progress.Stage(SelectedDevices, "ККТ: {0}"))
        foreach (var drive in progress.Stage(drives[kkt], "ФН: {0}"))
        foreach (var session in progress.Stage(await client.FetchSessionIdsByPeriodAsync(drive, date, date, cancellationToken), "Смена №{0}"))
        foreach(var doc in progress.Stage(await client.FetchDocumentNumbersBySessionAsync(drive, session, cancellationToken), "Документ №{0}"))
            if (await client.FetchReceiptAsync(drive, doc, cancellationToken) is { } receipt)
                yield return receipt;
        #pragma warning restore S3973
    }

    private async Task<Dictionary<string, string[]>> GetDriveByKktAsync
        (MultiProgress progress, CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, string[]>();
        using var _ = progress.Busy("Получение списка ФН...");
        foreach (var kkt in SelectedDevices)
            result.Add(kkt, await client.FetchDriveIdsAsync(kkt, cancellationToken).ToArrayAsync(cancellationToken));
        return result;
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
