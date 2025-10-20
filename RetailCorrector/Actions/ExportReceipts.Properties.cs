using Kraig.Attributes;
using RetailCorrector.Exports;

namespace RetailCorrector.Actions
{
    internal partial class ExportReceipts
    {
        public IExport Exporter => Exporters[ExporterId];
        [NotifyChanged] private int _exporterId;
        [NotifyChanged] private string _path;
    }
}
