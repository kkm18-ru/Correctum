using System.ComponentModel;

namespace RetailCorrector.Parsers
{
    internal partial class Sqlite : INotifyPropertyChanged
    {
        public string Path
        {
            get;
            set
            {
                if (value == field) return;
                field = value;
                PropertyChanged?.Invoke(this, PathNotifier);
            }
        } = "";

        private static readonly PropertyChangedEventArgs PathNotifier = new(nameof(Path));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
