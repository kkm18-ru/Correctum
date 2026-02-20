using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Importers;

[ImporterName("OFD.ru")]
public sealed partial class OfdRuImporter : ImporterBase
{
    [ImporterProperty("Начало периода")]
    public DateTime BeginDate { get; set => SetField(ref field, value); } = DateTime.Today;
    [ImporterProperty("Конец периода")]
    public DateTime EndDate { get; set => SetField(ref field, value); } = DateTime.Today;
    
    public bool Authorizing { get; set => SetField(ref field, value); } = false;
    public bool Authorized { get; set => SetField(ref field, value); } = false;
    
    public ObservableCollection<OfdRuDevice> Devices { get; } = [];
    [ImporterProperty("ККТ")]
    public ObservableCollection<OfdRuDevice> SelectedDevices { get; } = [];

    private OfdRuClient _client = new();
    private string Token
    {
        get => SecretHelper.DecodeSecret(field);
        set => SetField(ref field, SecretHelper.EncodeSecret(value));
    } = "";

    public OfdRuImporter()
    {
        InitializeComponent();
    }

    public override void OnClosing(CancelEventArgs e)
    {
        _client.Dispose();
        base.OnClosing(e);
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, Guid sessionId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        progress.ReportMaxValue(((int)(EndDate - BeginDate).TotalDays + 1) * SelectedDevices.Count);
        var i = 0;
        for (var day = BeginDate; day <= EndDate; day = day.AddDays(1))
        {
            foreach(var device in SelectedDevices)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var arr = await _client.FetchReceiptsAsync(sessionId, device, 
                    day, day.AddSeconds(86399), cancellationToken);
                foreach (var receipt in arr) yield return receipt;
                progress.ReportCurrValue(++i);
            }
        }
    }

    private void OnTokenChanged(object sender, RoutedEventArgs e) => Token = ((PasswordBox)sender).Password;

    private async void Auth(object sender, RoutedEventArgs e)
    {
        try
        {
            await Dispatcher.InvokeAsync(() => Authorizing = true);

            Devices.Clear();
            await _client.SetAuthTokenAsync(Token);
            foreach (var d in await _client.FetchDevicesAsync())
                Devices.Add(d);

            await Dispatcher.InvokeAsync(() => Authorized = true);
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
}