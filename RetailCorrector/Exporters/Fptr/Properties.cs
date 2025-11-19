using System.ComponentModel;

namespace RetailCorrector.Exporters
{
    internal partial class Fptr : INotifyPropertyChanged
    {
        #region Свойства
        public int CorrType
        {
            get => _isSelfCorr ? 0 : 1;
            set
            {
                if (_isSelfCorr == (value == 0)) return;
                _isSelfCorr = value == 0;
                PropertyChanged?.Invoke(this, CorrTypeNotifier);
            }
        }
        public int IsInternet
        {
            get => _isInternet ? 1 : 0;
            set
            {
                if (_isInternet == (value == 1)) return;
                _isInternet = value == 1;
                PropertyChanged?.Invoke(this, IsInternetNotifier);
            }
        }
        public string SiteAddress
        {
            get => _httpAddress;
            set
            {
                if (_httpAddress == value) return;
                _httpAddress = value;
                PropertyChanged?.Invoke(this, SiteAddressNotifier);
            }
        }
        public int PrintMode
        {
            get => _print ? 0 : 1;
            set
            {
                if (_print == (value == 0)) return;
                _print = value == 0;
                PropertyChanged?.Invoke(this, PrintModeNotifier);
            }
        }
        public string ActId
        {
            get => _actId;
            set
            {
                if (_actId == value) return;
                _actId = value;
                PropertyChanged?.Invoke(this, ActIdNotifier);
            }
        }
        public int FiscalFormat
        {
            get => _fiscalFormat;
            set
            {
                if (_fiscalFormat == value) return;
                _fiscalFormat = value;
                PropertyChanged?.Invoke(this, FiscalFormatNotifier);
            }
        }
        public int OldCorrReason
        {
            get => _wasReceipt ? 0 : 1;
            set
            {
                if (_wasReceipt == (value == 0)) return;
                _wasReceipt = value == 0;
                PropertyChanged?.Invoke(this, OldCorrReasonNotifier);
            }
        }
        #endregion

        #region Переменные
        private bool _isSelfCorr = true;
        private bool _isInternet = false;
        private string _httpAddress = "";
        private bool _print = false;
        private string _actId = "";
        private bool _wasReceipt = true;

        /// <summary>
        /// ФФД (0 - 1.2; 1 - 1.1; 2 - 1.05)
        /// </summary>
        private int _fiscalFormat = 0;
        #endregion

        #region Кеш PropertyChangedEventArgs
        private readonly static PropertyChangedEventArgs CorrTypeNotifier = new(nameof(CorrType));
        private readonly static PropertyChangedEventArgs IsInternetNotifier = new(nameof(IsInternet));
        private readonly static PropertyChangedEventArgs SiteAddressNotifier = new(nameof(SiteAddress));
        private readonly static PropertyChangedEventArgs PrintModeNotifier = new(nameof(PrintMode));
        private readonly static PropertyChangedEventArgs ActIdNotifier = new(nameof(ActId));
        private readonly static PropertyChangedEventArgs FiscalFormatNotifier = new(nameof(FiscalFormat));
        private readonly static PropertyChangedEventArgs OldCorrReasonNotifier = new(nameof(OldCorrReason));
        #endregion


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
