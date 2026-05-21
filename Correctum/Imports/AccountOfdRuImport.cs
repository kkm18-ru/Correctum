using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.Imports;

[ImportInfo("OFD.ru (через личный кабинет)")]
[TextBox("Логин", nameof(Login))]
[TextBox("Пароль", nameof(Password))]
[CheckComboBox("ККТ", nameof(SelectedDevices), nameof(Devices))]
[DateRange("Период", nameof(DateRange))]
[AuthorizationProperties(nameof(Login), nameof(Password))]
public sealed class AccountOfdRuImport : OfdImport<AccountOfdRuClient>
{
    public string Login { get; set => SetField(ref field, value); } = string.Empty;
    public string Password { get; set => SetField(ref field, value); } = string.Empty;
    public ObservableCollection<HtmlOfdRuKkt> SelectedDevices { get; } = [];
    public ObservableCollection<HtmlOfdRuKkt> Devices { get; } = [];
    public DateRange DateRange { get; } = new();

    private HtmlOfdRuUserIds UserIds { get; set; }

    public override async IAsyncEnumerable<Receipt> ImportAsync
        (BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var drives = new Dictionary<HtmlOfdRuKkt, string[]>();
        foreach (var kkt in SelectedDevices)
            drives.Add(kkt, [.. await client.GetDrivesAsync(UserIds.OfdAgreementId, kkt, cancellationToken), kkt.FnNumber]);

        progress.ReportMaximum((uint)(DateRange.Count * drives.SelectMany(i => i.Value).Count()));
        foreach(var date in DateRange)
        {
            foreach (var item in drives)
            {
                foreach (var drive in item.Value)
                {
                    await foreach (var doc in client.GetDocsInfoAsync
                        (UserIds.AccountId, UserIds.OfdAgreementId, item.Key.RegNumber, drive, 
                            date.ToDateTime(TimeOnly.MinValue), date.ToDateTime(TimeOnly.MaxValue), cancellationToken))
                        yield return await client.GetReceiptAsync(item.Key.Id, doc, cancellationToken);
                    progress.ReportIncrement();
                }
            }
        }
    }

    protected override async Task InternalAuthAsync()
    {
        SelectedDevices.Clear();
        Devices.Clear();
        await client.AuthAsync(Login, Password);
        UserIds = await client.GetUserIdsAsync();
        await foreach (var kkt in client.GetKktsAsync(UserIds.OfdAgreementId))
            Devices.Add(kkt);
    }
}
