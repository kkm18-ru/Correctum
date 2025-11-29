using System.ComponentModel;

namespace RetailCorrector.Exporters
{
    internal partial class Fptr : INotifyPropertyChanged
    {
        #region Свойства
        public int CorrType
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, CorrTypeNotifier);
            }
        } = 0;
        public bool IsInternet
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, IsInternetNotifier);
            }
        }
        public string SiteAddress
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, SiteAddressNotifier);
            }
        }
        public bool Print
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, PrintNotifier);
            }
        }
        public string ActId
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, ActIdNotifier);
            }
        }
        /// <summary>
        /// ФФД (0 - 1.2; 1 - 1.1; 2 - 1.05)
        /// </summary>
        public int FiscalFormat
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, FiscalFormatNotifier);
            }
        }
        public bool FixOldReceipts
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, FixOldReceiptsNotifier);
            }
        } = true;
        #endregion

        #region Кеш PropertyChangedEventArgs
        private readonly static PropertyChangedEventArgs CorrTypeNotifier = new(nameof(CorrType));
        private readonly static PropertyChangedEventArgs IsInternetNotifier = new(nameof(IsInternet));
        private readonly static PropertyChangedEventArgs SiteAddressNotifier = new(nameof(SiteAddress));
        private readonly static PropertyChangedEventArgs PrintNotifier = new(nameof(Print));
        private readonly static PropertyChangedEventArgs ActIdNotifier = new(nameof(ActId));
        private readonly static PropertyChangedEventArgs FiscalFormatNotifier = new(nameof(FiscalFormat));
        private readonly static PropertyChangedEventArgs FixOldReceiptsNotifier = new(nameof(FixOldReceipts));
        #endregion


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
