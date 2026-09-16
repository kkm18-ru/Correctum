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

[ImportInfo("Платформа ОФД")]
[DateRange("Период импортирования", nameof(DateRange))]
[CheckComboBox("ККТ", nameof(SelectedDevices), nameof(Devices))]
public sealed class PlatformOfdImport : OfdImport<PlatformaOfdClient>
{
    public DateRange DateRange { get; } = new();
    public ObservableCollection<PlatformaOfdDevice> Devices { get; } = [];
    public ObservableCollection<PlatformaOfdDevice> SelectedDevices { get; } = [];

    public override IAsyncEnumerable<Receipt> ImportAsync
        (MultiProgress progress, CancellationToken cancellationToken) =>
        DateStageAsync(progress, cancellationToken);

    private async Task<PlatformaOfdReceiptReference[]> GetDocReferencesAsync
        (DateOnly date, PlatformaOfdDevice kkt, MultiProgress progress,
        CancellationToken cancellationToken) 
    {
        using var busy = progress.Busy("Получение списка документов...");
        return await client.FetchReceiptsAsync(date, kkt, cancellationToken);
    }

    private async Task<PlatformaOfdReceiptReference[]> GetCorrDocReferencesAsync
        (DateOnly date, PlatformaOfdDevice kkt, MultiProgress progress,
        CancellationToken cancellationToken) 
    {
        using var busy = progress.Busy("Получение списка документов...");
        return await client.FetchCorrectionReceiptsAsync(date, kkt, cancellationToken);
    }
    
    private async IAsyncEnumerable<Receipt> DateStageAsync
        (MultiProgress progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.CreateStage(DateRange.Count);
        foreach (var date in DateRange)
        {
            progress.UpdateStageText($"Дата: {date:dd'.'MM'.'yyyy}");
            await foreach (var receipt in KktStageAsync(date, progress, cancellationToken))
                yield return receipt;
            progress.Increment();
        }
        progress.RemoveStage();
    }
    
    private async IAsyncEnumerable<Receipt> KktStageAsync
        (DateOnly date, MultiProgress progress,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        progress.CreateStage(SelectedDevices.Count);
        foreach (var kkt in SelectedDevices)
        {
            progress.UpdateStageText($"ККТ: {kkt}");
            PlatformaOfdReceiptReference[] references = [
                ..await GetDocReferencesAsync(date, kkt, progress, cancellationToken),
                ..await GetCorrDocReferencesAsync(date, kkt, progress, cancellationToken)];
            await foreach (var receipt in ReceiptStageAsync(references, progress, cancellationToken))
                yield return receipt;
            progress.Increment();
        }
        progress.RemoveStage();
    }
    
    private async IAsyncEnumerable<Receipt> ReceiptStageAsync
        (PlatformaOfdReceiptReference[] references, MultiProgress progress, 
        [EnumeratorCancellation] CancellationToken cancellationToken) 
    {
        progress.CreateStage(references.Length);
        foreach (var @ref in references)
        {
            progress.UpdateStageText($"Документ №{@ref.DocId}");
            if (await client.FetchReceiptAsync(@ref, cancellationToken) is { } receipt)
                yield return receipt;
            progress.Increment();
        }
        progress.RemoveStage();
    }

    protected override async Task InternalAuthAsync()
    {
        SelectedDevices.Clear();
        Devices.Clear();
        if (!await AuthorizationByBrowserAsync()) return;
        foreach (var d in await client.FetchDevicesAsync())
            Devices.Add(d);
    }

    private async Task<bool> AuthorizationByBrowserAsync()
    {
#pragma warning disable S1075
        try
        {
            using var browser = await PuppeteerHelper.OpenBrowser();
            var page = await browser.NewPageAsync();
            await page.GoToAsync("https://lk.platformaofd.ru/web/sso-login");
            await page.WaitForResponseAsync("https://lk.platformaofd.ru/web/multilogin/get-list/ajax?term=", new(300000));
            var cookies = await page.GetCookiesAsync();
            var sessionId = cookies.First(c => c.Name == "JSESSIONID").Value;
            var accessToken = cookies.First(c => c.Name == "ofd_access_token").Value;
            var refreshToken = cookies.First(c => c.Name == "refresh_token").Value;
            client.Auth(sessionId, accessToken, refreshToken);
            return true;
        }
        catch(PuppeteerSharp.TargetClosedException e) when(e.CloseReason == "Session closed")
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
