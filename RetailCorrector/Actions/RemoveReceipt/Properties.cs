using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Actions.RemoveReceipt
{
    internal partial class RemoveReceiptPresenter : INotifyPropertyChanged
    {
        public int FilterType
        {
            get => _isBlacklist ? 1 : 0;
            set
            {
                if (FilterType == value) return;
                _isBlacklist = value == 1;
                OnPropertyChanged();
            }
        }
        private bool _isBlacklist = false;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new(property));
    }
}
