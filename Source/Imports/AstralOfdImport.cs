using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
using Correctum.ViewModels.Controls;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Correctum.Imports;

[ImportInfo("Астрал ОФД")]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("Организации", nameof(SelectedOrganizations), nameof(Organizations))]
public sealed class AstralOfdImport : OfdImport<AstralOfdClient>
{
    public DateRange DateRange { get; } = new();
    public ObservableCollection<AstralOfdOrganization> Organizations { get; } = [];
    public ObservableCollection<AstralOfdOrganization> SelectedOrganizations { get; } = [];

    public override async IAsyncEnumerable<Receipt> ImportAsync
        (MultiProgress progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        #pragma warning disable S3973
        foreach (var date in progress.Stage(DateRange, "Дата: {0:dd'.'MM'.'yyyy}"))
        foreach (var org in progress.Stage(SelectedOrganizations, "Организация: {0}"))
        {
            var tickets = await GetTicketsAsync(date, org, progress, cancellationToken);
            foreach (var ticket in progress.Stage(tickets, "Документ №{0}"))
                yield return await client.GetReceiptAsync(ticket, cancellationToken);
        }
        #pragma warning restore S3973
    }

    private async Task<AstralOfdTicket[]> GetTicketsAsync
        (DateOnly date, AstralOfdOrganization org, MultiProgress progress,
        CancellationToken cancellationToken)
    {
        using var _ = progress.Busy("Получение списка документов...");
        return await client
            .EnumTicketInfoAsync(org, date.ToDateTime(TimeOnly.MinValue), cancellationToken)
            .Where(ticket => ticket.DocumentType.StartsWith('3'))
            .OrderBy(t => t.DocNumber).ToArrayAsync(cancellationToken);
    }

    protected override async Task InternalAuthAsync()
    {
        SelectedOrganizations.Clear();
        Organizations.Clear();
        if (!await AuthorizationByBrowserAsync()) return;
        
        await foreach (var org in client.EnumOrganizationAsync())
            Organizations.Add(org);
    }
    private async Task<bool> AuthorizationByBrowserAsync()
    {
        #pragma warning disable S1075
        try
        {
            using var browser = await PuppeteerHelper.OpenBrowser();
            var page = await browser.NewPageAsync();
            await page.GoToAsync("https://ofd.astralnalog.ru/lk/");
            await page.WaitForResponseAsync("https://ofd.astralnalog.ru/lk/api/v4.2/users.getMe", new(300000));
            var cookies = await page.GetCookiesAsync();
            var sessionId = cookies.First(c => c.Name == "connect.sid").Value;
            client.SetSessionId(sessionId);
            return true;
        }
        catch(PuppeteerSharp.TargetClosedException e) when (e.CloseReason == "Session closed")
        {
            logger.Warn(e, "Авторизация преждевременно завершено.");
            MessageBox.Show("Авторизация преждевременно завершено по причине: закрытие браузера");
            return false;
        }
        catch(TimeoutException e)
        {
            logger.Warn(e, "Превышено время ожидания ввода аутентификационных данных.");
            MessageBox.Show("Авторизация преждевременно завершено по причине: превышено время ожидания");
            return false;
        }
        #pragma warning restore S1075
    }
}
