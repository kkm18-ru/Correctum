using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using Correctum.ViewModels.Controls;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.Imports;

[ImportInfo("1-ОФД")]
[TextBox("Логин", nameof(Login))]
[TextBox("Пароль", nameof(Password))]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("ККТ", nameof(SelectedDevices), nameof(Devices))]
[AuthorizationProperties(nameof(Login), nameof(Password))]
public sealed class FirstOfdImport : OfdImport<FirstOfdClient>
{
    public string Login { get; set => SetField(ref field, value); } = string.Empty;
    public string Password { get; set => SetField(ref field, value); } = string.Empty;
    public DateRange DateRange { get; } = new();
    public ObservableCollection<FirstOfdKkt> Devices { get; } = [];
    public ObservableCollection<FirstOfdKkt> SelectedDevices { get; } = [];

    public override async IAsyncEnumerable<Receipt> ImportAsync
        (MultiProgress progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        #pragma warning disable S3973
        var dict = await GetDrivesAsync(progress, cancellationToken);
        foreach (var date in progress.Stage(DateRange, "Дата: {0:dd'.'MM'.'yyyy}"))
        foreach (var kkt in progress.Stage(SelectedDevices, "ККТ: {0}"))
        foreach (var drive in progress.Stage(dict[kkt], "ФН: {0}"))
        {
            if (drive.activationDate is null || drive.expireDate is null ||
                DateOnly.FromDateTime(drive.activationDate.Value) > DateRange.EndDate ||
                DateOnly.FromDateTime(drive.closeArchiveDate ?? drive.expireDate.Value) < DateRange.BeginDate) continue;

            var transactions = await GetTransactionsAsync(date, kkt, drive, progress, cancellationToken);
            foreach (var tr in progress.Stage(transactions, "Документ №{0}"))
                yield return await client.GetReceiptAsync(tr, cancellationToken);
        }
        #pragma warning restore S3973
    }

    private async Task<FirstOfdTransaction[]> GetTransactionsAsync
        (DateOnly date, FirstOfdKkt kkt, FirstOfdFiscalDrive drive,
        MultiProgress progress, CancellationToken cancellationToken)
    {
        using var _ = progress.Busy("Получение списка транзакций...");
        return await client.GetTransactionsAsync
            (kkt, date.ToDateTime(TimeOnly.MinValue), date.ToDateTime(TimeOnly.MaxValue),
            drive, Configuration.Current.Http.PageSize, cancellationToken)
            .OrderBy(tr => tr.fiscalDocumentNumber).ToArrayAsync(cancellationToken);
    }

    private async Task<Dictionary<FirstOfdKkt, FirstOfdFiscalDrive[]>> GetDrivesAsync
        (MultiProgress progress, CancellationToken cancellationToken)
    {
        using var _ = progress.Busy("Получение списка накопителей...");
        var result = new Dictionary<FirstOfdKkt, FirstOfdFiscalDrive[]>();
        foreach (var kkt in SelectedDevices)
            result[kkt] = await client.GetFiscalDrivesAsync(kkt, cancellationToken);
        return result;
    }

    protected override async Task InternalAuthAsync()
    {
        SelectedDevices.Clear();
        Devices.Clear();
        await client.AuthAsync(Login, Password);
        foreach (var placeId in await client.GetRetailPlaceIdsAsync())
            foreach (var kkt in await client.GetKktsAsync(placeId))
                Devices.Add(kkt);
    }
}
