using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Parsers
{
    internal partial class Mock : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        private int _minCountDocs = 1;
        public int MinCountDocs
        {
            get => _minCountDocs;
            set
            {
                if (_minCountDocs == value) return;
                _minCountDocs = value;
                OnPropertyChanged();
            }
        }

        private int _maxCountDocs = 1000;
        public int MaxCountDocs
        {
            get => _maxCountDocs;
            set
            {
                if (_maxCountDocs == value) return;
                _maxCountDocs = value;
                OnPropertyChanged();
            }
        }
    }
}
