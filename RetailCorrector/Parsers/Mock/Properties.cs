using System.ComponentModel;

namespace RetailCorrector.Parsers
{
    internal partial class Mock : INotifyPropertyChanged
    {
        public int MinCountDocs
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, MinCountDocsNotifier);
            }
        } = 1;
        public int MaxCountDocs
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, MaxCountDocsNotifier);
            }
        } = 1000;

        private static readonly PropertyChangedEventArgs MinCountDocsNotifier = new(nameof(MinCountDocs));
        private static readonly PropertyChangedEventArgs MaxCountDocsNotifier = new(nameof(MaxCountDocs));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
