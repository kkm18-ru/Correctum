using RetailCorrector.Exporters;
using RetailCorrector.Utils;
using System.ComponentModel;

namespace RetailCorrector.Actions
{
    internal partial class ExportReceipts : INotifyPropertyChanged
    {
        #region Свойства
        public IExporter Exporter
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, ExporterNotifier);
            }
        } = ModuleRegistry.Exporters[0];
        public string Path
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, PathNotifier);
            }
        } = "";
        #endregion

        #region Кеш PropertyChangedEventArgs
        private readonly static PropertyChangedEventArgs ExporterNotifier = new(nameof(Exporter));
        private readonly static PropertyChangedEventArgs PathNotifier = new(nameof(Path));
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
