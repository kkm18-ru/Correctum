using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace RetailCorrector.Parsers
{
    internal partial class Parser : INotifyPropertyChanged
    {
        private Func<Control> _generator = null!;
        public Func<Control> Generator
        {
            get => _generator;
            set
            {
                if (_generator == value) return;
                _generator = value;
                OnPropertyChanged();
                Child = _generator.Invoke();
                OnPropertyChanged(nameof(Child));
            }
        }

        public Control Child { get; set; } = new();

        public bool IsEnabledCancel => _cancelSource is not null && !_cancelSource.IsCancellationRequested;
        public bool IsEnabledParse => _cancelSource is null;

        private CancellationTokenSource? _cancelSource;
        public CancellationTokenSource? CancellationSource
        {
            get => _cancelSource;
            private set
            {
                _cancelSource = value;
                OnPropertyChanged(nameof(IsEnabledCancel));
                OnPropertyChanged(nameof(IsEnabledParse));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
