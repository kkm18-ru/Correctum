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
        var drives = await GetDrivesByKktAsync(progress, cancellationToken);
        await foreach (var receipt in DateStageAsync(drives, progress, cancellationToken))
            yield return receipt;
    }

    private async Task<Dictionary<HtmlOfdRuKkt, string[]>> GetDrivesByKktAsync
        (MultiProgress progress, CancellationToken cancellationToken)
    {
        var result = new Dictionary<HtmlOfdRuKkt, string[]>();
        using (progress.Busy("Получение списка ФН..."))
        {
            foreach (var kkt in SelectedDevices)
                result.Add(kkt, [.. await client.GetDrivesAsync(UserIds.OfdAgreementId, kkt, cancellationToken), kkt.FnNumber]);
        }
        return result;
    }

    private async IAsyncEnumerable<Receipt> DateStageAsync
        (Dictionary<HtmlOfdRuKkt, string[]> drives, MultiProgress progress,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.CreateStage(DateRange.Count);
        foreach (var date in DateRange)
        {
            progress.UpdateStageText($"Дата: {date:dd'.'MM'.'yyyy}");
            await foreach (var receipt in KktStageAsync(date, drives, progress, cancellationToken))
                yield return receipt;
            progress.Increment();
        }
        progress.RemoveStage();
    }

    private async IAsyncEnumerable<Receipt> KktStageAsync
        (DateOnly date, Dictionary<HtmlOfdRuKkt, string[]> drives, MultiProgress progress,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.CreateStage(drives.Count);
        foreach (var item in drives)
        {
            progress.UpdateStageText($"ККТ: {item.Key.RegNumber}");
            await foreach (var receipt in DriveStageAsync(date, item.Key, item.Value, progress, cancellationToken))
                yield return receipt;
            progress.Increment();
        }
        progress.RemoveStage();
    }

    private async IAsyncEnumerable<Receipt> DriveStageAsync
        (DateOnly date, HtmlOfdRuKkt kkt, string[] drives, MultiProgress progress, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.CreateStage(drives.Length);
        foreach (var drive in drives)
        {
            progress.UpdateStageText($"ФН: {drive}");
            var docs = await GetSummaryDocumentsAsync(kkt.RegNumber, drive, date, progress, cancellationToken);
            await foreach (var receipt in ReceiptStageAsync(kkt.Id, docs, progress, cancellationToken))
                yield return receipt;
            progress.Increment();
        }
        progress.RemoveStage();
    }

    private async IAsyncEnumerable<Receipt> ReceiptStageAsync
        (Guid kktId, HtmlOfdRuGetKktsDocumentsItem[] docs, MultiProgress progress, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.CreateStage(docs.Length);
        foreach (var doc in docs)
        {
            progress.UpdateStageText($"Документ №{doc.DocNumber}");
            yield return await client.GetReceiptAsync(kktId, doc, cancellationToken);
            progress.Increment();
        }
        progress.RemoveStage();
    }

    private async Task<HtmlOfdRuGetKktsDocumentsItem[]> GetSummaryDocumentsAsync
        (string regNumber, string drive, DateOnly date, MultiProgress progress, CancellationToken cancellationToken)
    {
        using (progress.Busy("Получение списка документов..."))
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
