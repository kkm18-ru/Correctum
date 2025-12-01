using RetailCorrector.Data;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations;
using System.ComponentModel;
using System.Windows.Input;

namespace RetailCorrector.Parsers;

internal sealed partial class FirstOfd : ParserBase<FirstOfd, FirstOfd.View>, INotifyPropertyChanged
{
    public override string Name => "Первый ОФД";

    #region Свойства
    public string ApiKey
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedApiKeyArgs);
        }
    } = "";

    public FirstOfdOrganisation[] Organizations
    {
        get;
        private set
        {
            if (value == field) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedOrganizationsArgs);
        }
    } = [];

    public string OrganizationId
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedOrganizationIdArgs);
            KKT = null;
            KKTs = string.IsNullOrWhiteSpace(value) ? [] :
                client!.FetchDevicesAsync(value)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }
    } = "";

    public FirstOfdKKT[] KKTs
    {
        get;
        private set
        {
            if (value == field) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedKKTsArgs);
        }
    } = [];

    public FirstOfdKKT? KKT
    {
        get;
        set
        {
            if (value.Equals(field)) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedKKTArgs);
            Drives = value?.Drives ?? [];
            DriveId = "";
        }
    } = null;

    public string[] Drives
    {
        get;
        private set
        {
            if (value == field) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedDrivesArgs);
        }
    } = [];

    public string DriveId
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedDriveIdArgs);
        }
    } = "";

    public DateTime BeginDate
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedBeginDateArgs);
        }
    } = DateTime.Today;

    public DateTime EndDate
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            PropertyChanged?.Invoke(this, CachedEndDateArgs);
        }
    } = DateTime.Today;
    #endregion

    #region Команды
    public ICommand AuthCommand => new RelayCommand(_ =>
    {
        client?.Dispose();
        client = new FirstOfdClient(ApiKey);
        Organizations = client.FetchOrganisationsAsync()
            .ConfigureAwait(false).GetAwaiter().GetResult();
        OrganizationId = "";
    });
    #endregion

    #region Кэш аргументов событий
    private static readonly PropertyChangedEventArgs CachedApiKeyArgs = new(nameof(ApiKey));
    private static readonly PropertyChangedEventArgs CachedOrganizationsArgs = new(nameof(Organizations));
    private static readonly PropertyChangedEventArgs CachedOrganizationIdArgs = new(nameof(OrganizationId));
    private static readonly PropertyChangedEventArgs CachedKKTsArgs = new(nameof(KKTs));
    private static readonly PropertyChangedEventArgs CachedKKTArgs = new(nameof(KKT));
    private static readonly PropertyChangedEventArgs CachedDrivesArgs = new(nameof(Drives));
    private static readonly PropertyChangedEventArgs CachedDriveIdArgs = new(nameof(DriveId));
    private static readonly PropertyChangedEventArgs CachedBeginDateArgs = new(nameof(BeginDate));
    private static readonly PropertyChangedEventArgs CachedEndDateArgs = new(nameof(EndDate));
    #endregion

    public event PropertyChangedEventHandler? PropertyChanged;
    private FirstOfdClient? client;

    public override ParseInfo GenerateInfo() =>
        parseInfo = new ParseInfo(Name, 
            ("ККТ", KKT!.Value.RegNumber), 
            ("ФН", DriveId), 
            ("Период", $"{BeginDate:d} - {EndDate:d}"));

    public override IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}