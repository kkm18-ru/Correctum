using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RetailCorrector.Parsers;

internal partial class Saby : INotifyPropertyChanged
{
    public string Login
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, LoginNotifier);
        }
    } = "";
    public string Password
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, PasswordNotifier);
        }
    } = "";
    public string Inn
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, InnNotifier);
            UpdateDevices();
        }
    } = "";
    public string Device
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, DeviceNotifier);
        }
    } = "";

    public ObservableCollection<string> Devices { get; set; } = [];
    public DateTime FromDate
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, FromDateNotifier);
        }
    } = DateTime.Today;
    public DateTime ToDate
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, ToDateNotifier);
        }
    } = DateTime.Today;
    public bool Authorizing
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, CanAuthorizeNotifier);
        }
    } = false;
    public bool Authorized
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, AuthorizedNotifier);
        }
    } = false;

    public bool CanAuthorize => !Authorizing;

    private int ProgressValue = 0;

#pragma warning disable S3168
    private async void UpdateDevices()
    {
        Devices.Clear();
        await foreach (var regId in _client.EnumerateDevicesAsync(Inn))
            Devices.Add(regId);
    }
#pragma warning restore S3168

    private readonly static PropertyChangedEventArgs LoginNotifier = new(nameof(Login));
    private readonly static PropertyChangedEventArgs PasswordNotifier = new(nameof(Password));
    private readonly static PropertyChangedEventArgs InnNotifier = new(nameof(Inn));
    private readonly static PropertyChangedEventArgs DeviceNotifier = new(nameof(Device));
    private readonly static PropertyChangedEventArgs FromDateNotifier = new(nameof(FromDate));
    private readonly static PropertyChangedEventArgs ToDateNotifier = new(nameof(ToDate));
    private readonly static PropertyChangedEventArgs CanAuthorizeNotifier = new(nameof(CanAuthorize));
    private readonly static PropertyChangedEventArgs AuthorizedNotifier = new(nameof(Authorized));

    public event PropertyChangedEventHandler? PropertyChanged;
}