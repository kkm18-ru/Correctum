using System.Collections.Immutable;

namespace RetailCorrector.Utils;

internal static class ModuleRegistry
{
    public static ImmutableArray<Deserializers.IReceiptDeserializer> Deserializers = [
        RetailCorrector.Deserializers.PlatformOfd.Instance,
        RetailCorrector.Deserializers.OfdRuJson.Instance,
    ];
    
    public static ImmutableArray<Exporters.IExporter> Exporters = [
        RetailCorrector.Exporters.Fptr.Instance,
        RetailCorrector.Exporters.Sqlite.Instance,
    ];
    
    public static ImmutableArray<Actions.IAction> Actions = [
        RetailCorrector.Actions.CancelReceipt.Instance,
        RetailCorrector.Actions.ExportReceipts.Instance,
        RetailCorrector.Actions.RemoveReceipt.Instance,
    ];
    
    public static ImmutableArray<Parsers.IParser> Parsers = [
        RetailCorrector.Parsers.Saby.Instance,
        RetailCorrector.Parsers.Sqlite.Instance,
        RetailCorrector.Parsers.OfdRu.Instance,
        RetailCorrector.Parsers.FxPOS.Instance,
        
        #if DEBUG
        RetailCorrector.Parsers.Mock.Instance,
        #endif
    ];
}