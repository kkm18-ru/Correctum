using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.Utils.Integrations.Clients;
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
        (BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.ReportMaximum((uint)(SelectedOrganizations.Count * DateRange.Count));
        foreach(var org in SelectedOrganizations)
        {
            foreach (var dt in DateRange)
            {
                var tickets = client
                    .EnumTicketInfoAsync(org, dt.ToDateTime(TimeOnly.MinValue), cancellationToken)
                    .Where(ticket => ticket.DocumentType.StartsWith('3'))
                    .WithCancellation(cancellationToken);

                await foreach (var ticket in tickets)
                    yield return await client.GetReceiptAsync(ticket, cancellationToken);
                progress.ReportIncrement();
            }
        }
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
