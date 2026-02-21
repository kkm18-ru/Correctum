using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.Exceptions.Saby;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Importers;

[ImporterName("SabyOFD")]
public sealed partial class SabyImporter : ImporterBase
{ 
    
    public string Login { get; set => SetField(ref field, value); } = "";
    public string Inn { get; set => SetField(ref field, value); } = "";
    
    [ImporterProperty("Начало периода")]
    public DateTime FromDate { get; set => SetField(ref field, value); } = DateTime.Now;
    [ImporterProperty("Конец периода")]
    public DateTime ToDate { get; set => SetField(ref field, value); } = DateTime.Now;
    
    public bool Authorizing { get; private set => SetField(ref field, value); } = false;
    public bool Authorized { get; private set => SetField(ref field, value); } = false;
    
    public ObservableCollection<string> Devices { get; set; } = [];
    [ImporterProperty("ККТ")]
    public ObservableCollection<string> SelectedDevices { get; set; } = [];

    private string Password
    {
        get => SecretHelper.DecodeSecret(field);
        set => SetField(ref field, SecretHelper.EncodeSecret(value));
    }

    private readonly SabyClient _client = new();

    public override bool CanStartClick =>
        !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password) &&
        Authorized && !string.IsNullOrWhiteSpace(Inn) && SelectedDevices.Count > 0;

    public SabyImporter()
    {
        InitializeComponent();
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, Guid sessionId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var drives = new Dictionary<string, SabyDriveResponse[]>();
        foreach (var device in SelectedDevices)
            drives.Add(device, await _client.FetchDrivesAsync(Inn, device, FromDate, ToDate, cancellationToken));

        progress.ReportMaxValue(drives.SelectMany(d => d.Value).Sum(d =>
        {
            var (start, end) = GetAvailablePeriod(d);
            return (end.Date - start.Date).Days + 1;
        }));
        foreach(var device in Devices)
        {
            foreach (var drive in drives[device])
            {
                await foreach (var receipt in ParseByStorage(device, sessionId, drive, progress, cancellationToken))
                    yield return receipt;
            }
        }
    }
    
    private async IAsyncEnumerable<Receipt> ParseByStorage(string regId, Guid sessionId, SabyDriveResponse drive, 
        VisualProgress progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var i = 0;
        var baseRequest = new SabyReceiptsRequest(sessionId, Inn, regId, drive.Id);
        var (start, end) = GetAvailablePeriod(drive);
        for(var day = start; day <= end; day = day.AddDays(1))
        {
            var request = baseRequest with { DateFrom = day, DateTo = day.AddSeconds(86_399) };
            await foreach (var receipt in ParseByDay(request, cancellationToken))
                yield return receipt;
            progress.ReportCurrValue(++i);
        }
    }

    private async IAsyncEnumerable<Receipt> ParseByDay
        (SabyReceiptsRequest args, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var offset = 0u;
        while (true)
        {
            var receipts = await _client.FetchReceiptsAsync(args, offset, cancellationToken);
            if (receipts is null || receipts.Length == 0) break;
            offset = receipts[^1].DocNumber;
            foreach (var receipt in receipts) yield return receipt;
        }
    }

    private (DateTime, DateTime) GetAvailablePeriod(SabyDriveResponse drive)
    {
        var end = drive.DateEnd ?? DateTime.Now;
        end = end > ToDate ? ToDate : end;
        var start = drive.DateStart > FromDate ? drive.DateStart : FromDate;
        return (start.Date, end.Date);
    }

    private async void Auth(object sender, RoutedEventArgs e)
    {
        try
        {
            await Dispatcher.InvokeAsync(() => Authorizing = true);
            await _client.AuthAsync(Login, Password).ConfigureAwait(false);
            await Dispatcher.InvokeAsync(() => Authorized = true);
        }
        catch (SabyAuthException ex)
        {
            await Dispatcher.InvokeAsync(() => Authorized = false);
            MessageBox.Show(ex.Description, ex.Title, 0, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            await Dispatcher.InvokeAsync(() => Authorized = false);
            MessageBox.Show(ex.Message, "Произошла ошибка...", 0, MessageBoxImage.Error);
        }
        finally
        {
            await Dispatcher.InvokeAsync(() => Authorizing = false);
        }
    }

    public override void OnClosing(CancelEventArgs e)
    {
        _client.Dispose();
        base.OnClosing(e);
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e) =>
        Password = ((PasswordBox)sender).Password;
}