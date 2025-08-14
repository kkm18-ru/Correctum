using RetailCorrector.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace RetailCorrector.Controls
{
    internal partial class ConditionGrid : INotifyPropertyChanged
    {
        public ObservableCollection<Condition> Conditions { get; } = [];

        private int _conditionIndex = -1;
        public int ConditionIndex
        {
            get => _conditionIndex;
            set
            {
                if (_conditionIndex == value) return;
                _conditionIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEnabledRemove));
                OnPropertyChanged(nameof(ForegroundRemove));
            }
        }
        public bool IsEnabledRemove => ConditionIndex != -1;
        public Brush ForegroundRemove => IsEnabledRemove ? Brushes.Red : Brushes.LightGray;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new(property));

        public bool Check(Receipt item)
        {
            return true;
        }
    }
}
