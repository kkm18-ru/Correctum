using System.ComponentModel;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu : INotifyPropertyChanged
    {
        public string Token
        {
            get => _token;
            set
            {
                if (_token == value) return;
                _token = value;
                PropertyChanged?.Invoke(this, TokenNotifier);
                TokenChanged?.Invoke();
            }
        }
        public string DeviceId
        {
            get => _deviceId;
            set
            {
                if (_deviceId == value) return;
                _deviceId = value;
                PropertyChanged?.Invoke(this, DeviceIdNotifier);
            }
        }
        public string Storage
        {
            get => _storage;
            set
            {
                if (_storage == value) return;
                _storage = value;
                PropertyChanged?.Invoke(this, StorageNotifier);
            }
        }
        public string Vatin
        {
            get => _vatin;
            set
            {
                if (_vatin == value) return;
                _vatin = value;
                PropertyChanged?.Invoke(this, VatinNotifier);
            }
        }
        public DateTime BeginDate
        {
            get => _beginDate;
            set
            {
                if (_beginDate == value) return;
                _beginDate = value;
                PropertyChanged?.Invoke(this, BeginDateNotifier);
            }
        }
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate == value) return;
                _endDate = value;
                PropertyChanged?.Invoke(this, EndDateNotifier);
            }
        }

        private string _token = "";
        private string _deviceId = "";
        private string _vatin = "";
        private string _storage = "";
        private DateTime _beginDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today;

        private readonly static PropertyChangedEventArgs TokenNotifier = new(nameof(Token));
        private readonly static PropertyChangedEventArgs DeviceIdNotifier = new(nameof(DeviceId));
        private readonly static PropertyChangedEventArgs StorageNotifier = new(nameof(Storage));
        private readonly static PropertyChangedEventArgs VatinNotifier = new(nameof(Vatin));
        private readonly static PropertyChangedEventArgs BeginDateNotifier = new(nameof(BeginDate));
        private readonly static PropertyChangedEventArgs EndDateNotifier = new(nameof(EndDate));

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? TokenChanged;
    }
}
