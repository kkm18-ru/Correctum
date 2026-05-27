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

[ImportInfo("OFD.ru")]
[TextBox("Токен", nameof(Token))]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("ККТ", nameof(SelectedDevices), nameof(Devices))]
[AuthorizationProperties(nameof(Token))]
public sealed class OfdRuImport : OfdImport<OfdRuClient>
{
    public string Token { get; set => SetField(ref field, value); } = string.Empty;
    public DateRange DateRange { get; } = new();
    public ObservableCollection<OfdRuDevice> Devices { get; } = [];
    public ObservableCollection<OfdRuDevice> SelectedDevices { get; } = [];

    public override async IAsyncEnumerable<Receipt> ImportAsync
        (MultiProgress progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        #pragma warning disable S3973
        foreach (var date in progress.Stage(DateRange, "Дата: {0:dd'.'MM'.'yyyy}"))
        foreach (var device in progress.Stage(SelectedDevices, "ККТ: {0}"))
        foreach (var receipt in await client.FetchReceiptsAsync(device, date, cancellationToken))
            yield return receipt;
        #pragma warning restore S3973
    }

    protected override async Task InternalAuthAsync()
    {
        SelectedDevices.Clear();
        Devices.Clear();

        await client.SetAuthTokenAsync(Token);

        foreach (var d in await client.FetchDevicesAsync())
            Devices.Add(d);
    }
}
