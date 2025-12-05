using System.Collections.Immutable;

namespace RetailCorrector.Utils;

internal static class ModuleRegistry
{
    public static ImmutableArray<Deserializers.IReceiptDeserializer> Deserializers = [
        RetailCorrector.Deserializers.PlatformOfd.Instance,
        RetailCorrector.Deserializers.OfdRuJson.Instance,
    ];
    
    public static ImmutableArray<Actions.IAction> Actions = [
        RetailCorrector.Actions.CancelReceipt.Instance,
        RetailCorrector.Actions.RemoveReceipt.Instance,
    ];
    
    public static ImmutableArray<Parsers.IParser> Parsers = [
        RetailCorrector.Parsers.Saby.Instance,
        RetailCorrector.Parsers.OfdRu.Instance,
        
        #if DEBUG
        RetailCorrector.Parsers.Mock.Instance,
        #endif
    ];
}