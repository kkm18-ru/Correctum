using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Models;
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
        #pragma warning disable S3973
        var drives = await GetDrivesByKktAsync(progress, cancellationToken);
        foreach (var date in progress.Stage(DateRange, "Дата: {0:dd'.'MM'.'yyyy}"))
        foreach (var kkt in progress.Stage(SelectedDevices, "ККТ: {0}"))
        foreach (var drive in progress.Stage(drives[kkt], "ФН: {0}"))
        {
            var docs = await GetSummaryDocumentsAsync(kkt.RegNumber, drive, date, progress, cancellationToken);
            foreach (var doc in progress.Stage(docs, "Документ №{0}"))
                yield return await client.GetReceiptAsync(kkt.Id, doc, cancellationToken);
        }
        #pragma warning restore S3973
    }

    private async Task<Dictionary<HtmlOfdRuKkt, string[]>> GetDrivesByKktAsync
        (MultiProgress progress, CancellationToken cancellationToken)
    {
        var result = new Dictionary<HtmlOfdRuKkt, string[]>();
        using var _ = progress.Busy("Получение списка ФН...");
        foreach (var kkt in SelectedDevices)
            result.Add(kkt, [.. await client.GetDrivesAsync(UserIds.OfdAgreementId, kkt, cancellationToken), kkt.FnNumber]);
        return result;
    }

    private async Task<HtmlOfdRuGetKktsDocumentsItem[]> GetSummaryDocumentsAsync
        (string regNumber, string drive, DateOnly date, MultiProgress progress, CancellationToken cancellationToken)
    {
        using var _ = progress.Busy("Получение списка документов...");
        return await client.GetDocsInfoAsync(UserIds.AccountId, UserIds.OfdAgreementId, regNumber, drive,
            date.ToDateTime(TimeOnly.MinValue), date.ToDateTime(TimeOnly.MaxValue), cancellationToken)
            .OrderBy(d => d.DocNumber).ToArrayAsync(cancellationToken);
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
