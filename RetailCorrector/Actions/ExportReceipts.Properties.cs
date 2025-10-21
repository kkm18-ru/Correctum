using Kraig.Attributes;
using RetailCorrector.Exports;
using RetailCorrector.Utils;

namespace RetailCorrector.Actions
{
    internal partial class ExportReceipts
    {
        [NotifyChanged] private IExport _exporter = ModuleRegistry.Exporters[0];
        [NotifyChanged] private string _path;
    }
}
