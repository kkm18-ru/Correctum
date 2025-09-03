using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Parsers
{
    internal partial class FxPOS : INotifyPropertyChanged
    {
        public string Path
        {
            get => _path;
            set
            {
                if (_path == value) return;
                _path = value;
                OnPropertyChanged();
            }
        }

        private string _path = "";

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
