using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.Exceptions;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;
using Range = (string drive, uint min, uint max);

namespace RetailCorrector.Importers;

[ImporterName("Такском-Касса")]
public sealed partial class TaxcomImporter : ImporterBase
{
    public string Login { get; set => SetField(ref field, value); } = string.Empty;
    public string Agreement { get; set => SetField(ref field, value); } = string.Empty;
    [ImporterProperty("Начало периода")]
    public DateTime BeginDate { get; set => SetField(ref field, value); } = DateTime.Now;
    [ImporterProperty("Конец периода")]
    public DateTime EndDate { get; set => SetField(ref field, value); } = DateTime.Now;
    public bool Authorizing { get; private set => SetField(ref field, value); } = false;
    public bool Authorized { get; private set => SetField(ref field, value); } = false;

    public ObservableCollection<string> Devices { get; } = [];
    [ImporterProperty("ККТ")]
    public ObservableCollection<string> SelectedDevices { get; } = [];

    private string _password
    {
        get => SecretHelper.DecodeSecret(field);
        set => SetField(ref field, SecretHelper.EncodeSecret(value));
    } = "";
    private readonly TaxcomClient _client = new();
    
    public TaxcomImporter()
    {
        InitializeComponent();
    }

    private async void Auth(object? sender, RoutedEventArgs args)
    {
        try
        {
            await Dispatcher.InvokeAsync(() => SelectedDevices.Clear());
            await Dispatcher.InvokeAsync(() => Devices.Clear());
                
            await Dispatcher.InvokeAsync(() => Authorizing = true);
            await _client.AuthAsync(Login, _password, Agreement);
            await foreach (var outlet in _client.FetchOutletIdsAsync())
            {
                await foreach (var device in _client.FetchDevicesAsync(outlet))
                    await Dispatcher.InvokeAsync(() => Devices.Add(device));
            }
            await Dispatcher.InvokeAsync(() => Authorized = true);
        }
        catch (Exception e)
        {
            await Dispatcher.InvokeAsync(() => Authorized = false);
            if (e is TaxcomException te)
                MessageBox.Show(te.Message, te.Summary, 0, MessageBoxImage.Error);
            else MessageBox.Show(e.ToString(), null, 0, MessageBoxImage.Error);
        }
        finally
        {
            await Dispatcher.InvokeAsync(() => Authorizing = false);
        }
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, Guid sessionId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var ranges = new List<Range>(SelectedDevices.Count);

        foreach (var device in SelectedDevices)
        {
            await foreach (var drive in _client.FetchDriveIdsAsync(device, cancellationToken))
            {
                var sessionIds = await _client.FetchSessionIdsByPeriodAsync
                    (drive, BeginDate, EndDate, cancellationToken);
                if(sessionIds.Count == 0) continue;
                sessionIds.Sort();
                var docIds = await _client.FetchDocumentNumbersBySessionAsync(drive, sessionIds[0], cancellationToken);
                var minDocId = docIds[0];
                if (sessionIds.Count > 1)
                    docIds = await _client.FetchDocumentNumbersBySessionAsync(drive, sessionIds[^1], cancellationToken);
                var maxDocId = docIds[^1];
                ranges.Add((drive, minDocId, maxDocId));
                progress.ReportMaxValue((int)ranges.Sum(r => r.max - r.min + 1));
            }
        }

        var i = 0;
        foreach (var range in ranges)
        {
            for (var j = range.min; j <= range.max; j++)
            {
                if (await _client.FetchReceiptAsync(range.drive, j, sessionId, cancellationToken) is { } receipt)
                    yield return receipt;
                progress.ReportCurrValue(++i);
            }
        }
    }

    public override void OnClosing(CancelEventArgs e)
    {
        _client.Dispose();
        base.OnClosing(e);
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e) => 
        _password = ((PasswordBox)sender).Password;
}