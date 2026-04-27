using RetailCorrector.Utils;
using System.Windows.Input;

namespace RetailCorrector.ViewModels
{
    public sealed class ReceiptGridViewModel : BaseViewModel
    {
        public uint CurrentPage
        {
            get => field;
            set
            {
                if (field == value) return;
                if (value == 0)
                {
                    OnPropertyChanged();
                    return;
                }
                field = value;
                OnPropertyChanged();
            }
        } = 1;
        public static uint CountPage => Storage.Instance is null ? 0 : (uint)Math.Ceiling(Storage.Instance.AllReceipts.Count() / 100D);

        public ICommand RefreshCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }

        public ReceiptGridViewModel()
        {
            RefreshCommand = new RelayCommand(() => Storage.Instance.UpdateReceiptData(CurrentPage, 100), null);
            FirstPageCommand = new RelayCommand(() =>
            {
                CurrentPage = 1;
                Storage.Instance.UpdateReceiptData(CurrentPage, 100);
            }, () => CurrentPage > 1);
            PrevPageCommand = new RelayCommand(() => Storage.Instance.UpdateReceiptData(--CurrentPage, 100), 
                () => CurrentPage > 1);
            NextPageCommand = new RelayCommand(() => Storage.Instance.UpdateReceiptData(++CurrentPage, 100), 
                () => CurrentPage < CountPage);
            LastPageCommand = new RelayCommand(() =>
            {
                CurrentPage = CountPage;
                Storage.Instance.UpdateReceiptData(CurrentPage, 100);
            }, () => CurrentPage < CountPage);
        }
    }
}
