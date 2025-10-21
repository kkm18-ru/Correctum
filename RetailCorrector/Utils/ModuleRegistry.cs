using System.Collections.Immutable;

namespace RetailCorrector.Utils;

internal static class ModuleRegistry
{
    public static ImmutableArray<Deserializers.IReceiptDeserializer> Deserializers = [
        RetailCorrector.Deserializers.PlatformOfd.Instance,
    ];
    
    public static ImmutableArray<Exports.IExport> Exporters = [
        Exports.Fptr.Instance,
        Exports.Sqlite.Instance,
    ];
    
    public static ImmutableArray<Actions.IAction> Actions = [
        RetailCorrector.Actions.CancelReceipt.Instance,
        RetailCorrector.Actions.ExportReceipts.Instance,
        RetailCorrector.Actions.RemoveReceipt.Instance,
    ];
    
    public static ImmutableArray<Parsers.IParser> Parsers = [
        RetailCorrector.Parsers.Mock.Instance,
        RetailCorrector.Parsers.Saby.Instance,
        RetailCorrector.Parsers.Sqlite.Instance,
        RetailCorrector.Parsers.OfdRu.Instance,
        RetailCorrector.Parsers.FxPOS.Instance
    ];
}