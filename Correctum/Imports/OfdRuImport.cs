using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
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
        ([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        //progress.ReportMaximum((uint)(DateRange.Count * SelectedDevices.Count));
        foreach (var day in DateRange)
        {
            foreach (var device in SelectedDevices)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var arr = await client.FetchReceiptsAsync(device, day, cancellationToken);
                foreach (var receipt in arr) yield return receipt;
                //progress.ReportIncrement();
            }
        }
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
