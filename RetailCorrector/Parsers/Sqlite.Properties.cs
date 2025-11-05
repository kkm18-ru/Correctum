using System.ComponentModel;

namespace RetailCorrector.Parsers
{
    internal partial class Sqlite: INotifyPropertyChanged
    {
        public string Path
        {
            get => _path;
            set
            {
                if (value == _path) return;
                _path = value;
                PropertyChanged?.Invoke(this, PathNotifier);
            }
        }

        private string _path = "";

        private static readonly PropertyChangedEventArgs PathNotifier = new(nameof(Path));

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
