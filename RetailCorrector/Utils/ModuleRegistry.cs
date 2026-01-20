using System.Collections.Immutable;

namespace RetailCorrector.Utils;

internal static class ModuleRegistry
{
    public readonly static ImmutableArray<Parsers.IParser> Parsers = [
        RetailCorrector.Parsers.PlatformOfd.Instance,
        RetailCorrector.Parsers.OfdRuJson.Instance,
    ];

    public readonly static ImmutableArray<Exporters.IExporter> Exporters = [
        RetailCorrector.Exporters.Fptr.Instance,
        RetailCorrector.Exporters.Sqlite.Instance,
    ];

    public readonly static ImmutableArray<Importers.IImporter> Importers = [
        RetailCorrector.Importers.Saby.Instance,
        RetailCorrector.Importers.Sqlite.Instance,
        RetailCorrector.Importers.OfdRu.Instance,
        RetailCorrector.Importers.PlatformOfd.Instance,
        RetailCorrector.Importers.FxPOS.Instance,
        RetailCorrector.Importers.Acquirer.Instance,
        
        #if DEBUG
        RetailCorrector.Importers.Mock.Instance,
        #endif
    ];
}