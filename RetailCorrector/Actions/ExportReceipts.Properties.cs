using RetailCorrector.Attributes;
using RetailCorrector.Exports;

namespace RetailCorrector.Actions
{
    internal partial class ExportReceipts
    {
        public IExport Exporter => Exporters[ExporterId];
        [NotifyChanged] public partial int ExporterId { get; set; }
        [NotifyChanged] public partial string Path { get; set; }
    }
}
