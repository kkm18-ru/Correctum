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
        (MultiProgress progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var drives = new Dictionary<HtmlOfdRuKkt, string[]>();
        foreach (var kkt in SelectedDevices)
            drives.Add(kkt, [.. await client.GetDrivesAsync(UserIds.OfdAgreementId, kkt, cancellationToken), kkt.FnNumber]);

        progress.AddStage(DateRange.Count);
        foreach(var date in DateRange)
        {
            progress.SetStageText(0, $"Дата: {date:dd'.'MM'.'yyyy}");
            progress.AddStage(drives.Count);
            foreach (var item in drives)
            {
                progress.SetStageText(1, $"ККТ: {item.Key.RegNumber}");
                progress.AddStage(item.Value.Length);
                foreach (var drive in item.Value)
                {
                    progress.SetStageText(2, $"ФН: {drive}");
                    var collection = await client.GetDocsInfoAsync(UserIds.AccountId, UserIds.OfdAgreementId, item.Key.RegNumber, drive,
                        date.ToDateTime(TimeOnly.MinValue), date.ToDateTime(TimeOnly.MaxValue), cancellationToken).ToArrayAsync(cancellationToken);
                    progress.AddStage(collection.Length);
                    foreach (var doc in collection)
                    {
                        progress.SetStageText(3, $"Документ №{doc.DocNumber}");
                        yield return await client.GetReceiptAsync(item.Key.Id, doc, cancellationToken);
                        progress.Increment(3);
                    }
                    progress.RemoveStage();
                    progress.Increment(2);
                }
                progress.RemoveStage();
                progress.Increment(1);
            }
            progress.RemoveStage();
            progress.Increment(0);
        }
        progress.RemoveStage();
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
