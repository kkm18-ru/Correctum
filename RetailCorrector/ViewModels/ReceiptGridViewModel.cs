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
        public static uint CountPage => Storage.Instance is null ? 0 : (uint)Math.Ceiling(Storage.Instance.Receipts.Count() / 100D);

        public ICommand RefreshCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }

        public ReceiptGridViewModel()
        {
            RefreshCommand = new RelayCommand(_ => Storage.Instance.UpdateReceiptData(CurrentPage, 100), null);
            FirstPageCommand = new RelayCommand(_ =>
            {
                CurrentPage = 1;
                Storage.Instance.UpdateReceiptData(CurrentPage, 100);
            }, _ => CurrentPage > 1);
            PrevPageCommand = new RelayCommand(_ => Storage.Instance.UpdateReceiptData(--CurrentPage, 100), 
                _ => CurrentPage > 1);
            NextPageCommand = new RelayCommand(_ => Storage.Instance.UpdateReceiptData(++CurrentPage, 100), 
                _ => CurrentPage < CountPage);
            LastPageCommand = new RelayCommand(_ =>
            {
                CurrentPage = CountPage;
                Storage.Instance.UpdateReceiptData(CurrentPage, 100);
            }, _ => CurrentPage < CountPage);
        }
    }
}
