using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.Imports;

[ImportInfo("Платформа ОФД")]
[TextBox("Идентификатор сессии", nameof(SessionId))]
[TextBox("Токен доступа", nameof(AccessToken))]
[TextBox("Токен обновления", nameof(RefreshToken))]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("ККТ", nameof(Devices), nameof(SelectedDevices))]
[AuthorizationProperties(nameof(SessionId), nameof(AccessToken), nameof(RefreshToken))]
public sealed class PlatformOfdImport : OfdImport<PlatformaOfdClient>
{
    public string SessionId { get; set => SetField(ref field, value); } = string.Empty;
    public string AccessToken { get; set => SetField(ref field, value); } = string.Empty;
    public string RefreshToken { get; set => SetField(ref field, value); } = string.Empty;
    public DateRange DateRange { get; } = new();
    public ObservableCollection<PlatformaOfdDevice> Devices { get; } = [];
    public ObservableCollection<PlatformaOfdDevice> SelectedDevices { get; } = [];

    public override async IAsyncEnumerable<Receipt> ImportAsync
        ([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var date in DateRange)
        {
            foreach (var Device in SelectedDevices)
            {
                var references = await client.FetchReceiptsAsync(date, Device, cancellationToken);
                foreach (var @ref in references)
                    if (await client.FetchReceiptAsync(@ref, cancellationToken) is { } receipt)
                        yield return receipt;
            }
        }
    }

    protected override async Task InternalAuthAsync()
    {
        SelectedDevices.Clear();
        Devices.Clear();

        client.Auth(SessionId, AccessToken, RefreshToken);
        foreach (var d in await client.FetchDevicesAsync())
            Devices.Add(d);
    }
}
