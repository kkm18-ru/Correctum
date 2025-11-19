using System.ComponentModel;

namespace RetailCorrector.Parsers
{
    internal partial class Mock : INotifyPropertyChanged
    {
        public int MinCountDocs
        {
            get => _minCountDocs;
            set
            {
                if (_minCountDocs == value) return;
                _minCountDocs= value;
                PropertyChanged?.Invoke(this, MinCountDocsNotifier);
            }
        }
        public int MaxCountDocs
        {
            get => _maxCountDocs;
            set
            {
                if (_maxCountDocs == value) return;
                _maxCountDocs = value;
                PropertyChanged?.Invoke(this, MaxCountDocsNotifier);
            }
        }

        private int _minCountDocs = 1;
        private int _maxCountDocs = 1000;

        private static readonly PropertyChangedEventArgs MinCountDocsNotifier = new(nameof(MinCountDocs));
        private static readonly PropertyChangedEventArgs MaxCountDocsNotifier = new(nameof(MaxCountDocs));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
