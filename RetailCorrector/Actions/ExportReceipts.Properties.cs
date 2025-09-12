using RetailCorrector.Exports;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Actions
{
    internal partial class ExportReceipts : INotifyPropertyChanged
    {
        public IExport Exporter
        {
            get => _export;
            set
            {
                if (_export == value) return;
                _export = value;
                OnPropertyChanged();
                if (View.grid.RowDefinitions.Count > 1)
                {
                    View.grid.RowDefinitions.RemoveRange(1, View.grid.RowDefinitions.Count - 1);
                    View.grid.Children.RemoveRange(2, View.grid.Children.Count - 2);
                }
                _export.InitVisual(View.grid);
            }
        }
        private IExport _export = Exporters[0];

        public readonly static IExport[] Exporters = [ Sqlite.Instance, Fptr.Instance ];

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
