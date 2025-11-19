using RetailCorrector.Exports;
using RetailCorrector.Utils;
using System.ComponentModel;

namespace RetailCorrector.Actions
{
    internal partial class ExportReceipts : INotifyPropertyChanged
    {
        #region Свойства
        public IExport Exporter
        {
            get => _exporter;
            set
            {
                if (_exporter == value) return;
                _exporter = value;
                PropertyChanged?.Invoke(this, ExporterNotifier);
            }
        }
        public string Path
        {
            get => _path;
            set
            {
                if (_path == value) return;
                _path = value;
                PropertyChanged?.Invoke(this, PathNotifier);
            }
        }
        #endregion

        #region Переменные
        private IExport _exporter = ModuleRegistry.Exporters[0];
        private string _path = "";
        #endregion

        #region Кеш PropertyChangedEventArgs
        private readonly static PropertyChangedEventArgs ExporterNotifier = new(nameof(Exporter));
        private readonly static PropertyChangedEventArgs PathNotifier = new(nameof(Path));
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
