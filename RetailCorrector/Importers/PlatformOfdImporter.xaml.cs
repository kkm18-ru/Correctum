using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using RetailCorrector.Data;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;

namespace RetailCorrector.Importers;

public sealed partial class PlatformOfdImporter : ImporterBase
{
    public new static string FormName { get; } = "Платформа ОФД";
    
    public string SessionId { get; set => SetField(ref field, value); } = "";
    
    public bool Authorizing { get; set => SetField(ref field, value); } = false;
    public bool Authorized { get; set => SetField(ref field, value); } = false;
    
    public DateTime BeginDate { get; set => SetField(ref field, value); } = DateTime.Now;
    public DateTime EndDate { get; set => SetField(ref field, value); } = DateTime.Now;
    
    public ObservableCollection<PlatformaOfdDevice> Devices { get; } = [];
    public ObservableCollection<PlatformaOfdDevice> SelectedDevices { get; } = [];

    private readonly PlatformaOfdClient _client = new();

    private string AccessToken
    {
        get => SecretHelper.DecodeSecret(field); 
        set => SetField(ref field, SecretHelper.EncodeSecret(value));
    } = "";

    private string RefreshToken
    {
        get => SecretHelper.DecodeSecret(field); 
        set => SetField(ref field, SecretHelper.EncodeSecret(value));
    } = "";

    public PlatformOfdImporter()
    {
        InitializeComponent();
    }

    public override void OnClosing(CancelEventArgs e)
    {
        _client.Dispose();
        base.OnClosing(e);
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        progress.ReportMaxValue((int)Math.Ceiling((EndDate - BeginDate).TotalDays / 7) * SelectedDevices.Count);
        for (var (d, i) = (BeginDate, 0); d < EndDate; d = d.AddDays(7), i++)
        {
            foreach(var Device in SelectedDevices)
            {
                var end = d.AddSeconds(604_799);
                end = end > EndDate ? EndDate : end;
                var references = await _client.FetchReceiptsAsync(d, end, Device, cancellationToken);
                foreach (var @ref in references)
                    if(await _client.FetchReceiptAsync(@ref, Guid.Empty, cancellationToken) is {} receipt) // todo
                        yield return receipt;
                progress.ReportCurrValue(i);
            }
        }
    }

    private void OnAccessTokenChanged(object sender, RoutedEventArgs e) =>
        AccessToken = ((PasswordBox)sender).Password;
    private void OnRefreshTokenChanged(object sender, RoutedEventArgs e) =>
        RefreshToken = ((PasswordBox)sender).Password;

    private async void Auth(object sender, RoutedEventArgs e)
    {
        try
        {
            await Dispatcher.InvokeAsync(() => Authorizing = true);

            Devices.Clear();
            _client.Auth(SessionId, AccessToken, RefreshToken);
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