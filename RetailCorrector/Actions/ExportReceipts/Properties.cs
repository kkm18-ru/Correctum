using RetailCorrector.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Actions.ExportReceipts
{
    internal partial class ExportReceiptsPresenter : INotifyPropertyChanged
    {
        public int Type
        {
            get => (int)_type;
            set
            {
                if ((int)_type != value) return;
                _type = (ExportDestination)value;
                OnPropertyChanged();
            }
        }
        private ExportDestination _type;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
