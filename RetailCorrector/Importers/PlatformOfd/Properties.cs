using RetailCorrector.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RetailCorrector.Importers;

partial class PlatformOfd : INotifyPropertyChanged
{
    public string SessionId
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, SessionIdEventArg);
        }
    } = "";
    public string AccessToken
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, AccessTokenEventArg);
        }
    } = "";
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
    public DateTime BeginDate
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, BeginDateNotifier);
        }
    } = DateTime.Today;
    public DateTime EndDate
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, EndDateNotifier);
        }
    } = DateTime.Today;

    public bool CanAuthorize => !Authorizing;
    public ObservableCollection<PlatformaOfdDevice> Devices { get; } = [];
    public ObservableCollection<PlatformaOfdDevice> SelectedDevices { get; } = [];

    private readonly static PropertyChangedEventArgs SessionIdEventArg = new(nameof(SessionId));
    private readonly static PropertyChangedEventArgs AccessTokenEventArg = new(nameof(AccessToken));
    private readonly static PropertyChangedEventArgs BeginDateNotifier = new(nameof(BeginDate));
    private readonly static PropertyChangedEventArgs EndDateNotifier = new(nameof(EndDate));
    private readonly static PropertyChangedEventArgs CanAuthorizeNotifier = new(nameof(CanAuthorize));
    private readonly static PropertyChangedEventArgs AuthorizedNotifier = new(nameof(Authorized));

    public event PropertyChangedEventHandler? PropertyChanged;
}
