using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
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
        (BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var drives = new List<(FirstOfdKkt, FirstOfdFiscalDrive)>();
        foreach (var kkt in SelectedDevices)
            foreach (var fn in await client.GetFiscalDrivesAsync(kkt, cancellationToken))
                drives.Add((kkt, fn));

        progress.ReportMaximum((uint)drives.Count);
        foreach(var (kkt, fn) in drives)
        {
            if (fn.activationDate is null || fn.expireDate is null ||
                DateOnly.FromDateTime(fn.activationDate.Value) > DateRange.EndDate ||
                DateOnly.FromDateTime(fn.closeArchiveDate ?? fn.expireDate.Value) < DateRange.BeginDate) continue;
            var fromDate = DateRange.BeginDate.ToDateTime(TimeOnly.MinValue);
            var toDate = DateRange.EndDate.ToDateTime(TimeOnly.MaxValue);
            var size = Configuration.Current.Http.PageSize;
            var enumerable = client.GetTransactionsAsync(kkt, fromDate, toDate, fn, size, cancellationToken);
            await foreach (var tr in enumerable.WithCancellation(cancellationToken))
                yield return await client.GetReceiptAsync(tr, cancellationToken);
            progress.ReportIncrement();
        }
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
