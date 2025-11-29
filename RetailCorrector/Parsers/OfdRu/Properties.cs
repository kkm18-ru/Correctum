using System.ComponentModel;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu : INotifyPropertyChanged
    {
        public string Token
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, TokenNotifier);
                TokenChanged?.Invoke();
            }
        } = "";
        public string DeviceId
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, DeviceIdNotifier);
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
        public string Vatin
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, VatinNotifier);
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
