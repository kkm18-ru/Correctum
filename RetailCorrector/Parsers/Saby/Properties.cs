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
    public string Storage
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, StorageNotifier);
        }
    } = "";
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

    private readonly static PropertyChangedEventArgs LoginNotifier = new(nameof(Login));
    private readonly static PropertyChangedEventArgs PasswordNotifier = new(nameof(Password));
    private readonly static PropertyChangedEventArgs InnNotifier = new(nameof(Inn));
    private readonly static PropertyChangedEventArgs DeviceNotifier = new(nameof(Device));
    private readonly static PropertyChangedEventArgs StorageNotifier = new(nameof(Storage));
    private readonly static PropertyChangedEventArgs FromDateNotifier = new(nameof(FromDate));
    private readonly static PropertyChangedEventArgs ToDateNotifier = new(nameof(ToDate));

    public event PropertyChangedEventHandler? PropertyChanged;
}