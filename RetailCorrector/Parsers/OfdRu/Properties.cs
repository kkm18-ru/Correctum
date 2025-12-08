using RetailCorrector.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu : INotifyPropertyChanged
    {
        public OfdRuDevice Device
        {
            get;
            set
            {
                if (field.Equals(value)) return;
                field = value;
                PropertyChanged?.Invoke(this, DeviceNotifier);
            }
        }
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
        public string Token
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, TokenNotifier);
            }
        } = "";
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

        public ObservableCollection<OfdRuDevice> Devices { get; } = [];

        private readonly static PropertyChangedEventArgs DeviceNotifier = new(nameof(Device));
        private readonly static PropertyChangedEventArgs StorageNotifier = new(nameof(Storage));
        private readonly static PropertyChangedEventArgs TokenNotifier = new(nameof(Token));
        private readonly static PropertyChangedEventArgs BeginDateNotifier = new(nameof(BeginDate));
        private readonly static PropertyChangedEventArgs EndDateNotifier = new(nameof(EndDate));
        private readonly static PropertyChangedEventArgs CanAuthorizeNotifier = new(nameof(CanAuthorize));
        private readonly static PropertyChangedEventArgs AuthorizedNotifier = new(nameof(Authorized));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
