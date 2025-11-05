using System.ComponentModel;

namespace RetailCorrector.Parsers;

internal partial class Saby: INotifyPropertyChanged
{
    public string Login
    {
        get => _login;
        set
        {
            if(_login == value) return;
            _login = value;
            PropertyChanged?.Invoke(this, LoginNotifier);
        }
    }
    public string Password
    {
        get => _password;
        set
        {
            if(_password == value) return;
            _password = value;
            PropertyChanged?.Invoke(this, PasswordNotifier);
        }
    }
    public string Inn
    {
        get => _inn;
        set
        {
            if(_inn == value) return;
            _inn = value;
            PropertyChanged?.Invoke(this, InnNotifier);
        }
    }
    public string Device
    {
        get => _device;
        set
        {
            if(_device == value) return;
            _device = value;
            PropertyChanged?.Invoke(this, DeviceNotifier);
        }
    }
    public string Storage
    {
        get => _storage;
        set
        {
            if(_storage == value) return;
            _storage = value;
            PropertyChanged?.Invoke(this, StorageNotifier);
        }
    }
    public DateTime FromDate
    {
        get => _fromDate;
        set
        {
            if(_fromDate == value) return;
            _fromDate = value;
            PropertyChanged?.Invoke(this, FromDateNotifier);
        }
    }
    public DateTime ToDate
    {
        get => _toDate;
        set
        {
            if(_toDate == value) return;
            _toDate = value;
            PropertyChanged?.Invoke(this, ToDateNotifier);
        }
    }

    private string _login = "";
    private string _password = "";
    private string _inn = "";
    private string _device = "";
    private string _storage = "";
    private DateTime _fromDate = DateTime.Today;
    private DateTime _toDate = DateTime.Today;

    private readonly static PropertyChangedEventArgs LoginNotifier = new(nameof(Login));
    private readonly static PropertyChangedEventArgs PasswordNotifier = new(nameof(Password));
    private readonly static PropertyChangedEventArgs InnNotifier = new(nameof(Inn));
    private readonly static PropertyChangedEventArgs DeviceNotifier = new(nameof(Device));
    private readonly static PropertyChangedEventArgs StorageNotifier = new(nameof(Storage));
    private readonly static PropertyChangedEventArgs FromDateNotifier = new(nameof(FromDate));
    private readonly static PropertyChangedEventArgs ToDateNotifier = new(nameof(ToDate));

    public event PropertyChangedEventHandler? PropertyChanged;
}