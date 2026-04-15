using System.Collections.Immutable;
using RetailCorrector.UI.Controls;

namespace RetailCorrector.Utils;

internal static class ModuleRegistry
{
    public static readonly ImmutableArray<Parsers.IParser> Parsers = [
        RetailCorrector.Parsers.PlatformOfd.Instance,
        RetailCorrector.Parsers.OfdRuJson.Instance,
    ];

    public static readonly ImmutableArray<ImportMenuItem> Importers = [
        new(RetailCorrector.Importers.Saby.Instance),
        new(RetailCorrector.Importers.Sqlite.Instance),
        new(RetailCorrector.Importers.OfdRu.Instance),
        new(RetailCorrector.Importers.PlatformOfd.Instance),
        new(RetailCorrector.Importers.FxPOS.Instance),
        new(RetailCorrector.Importers.Acquirer.Instance),
        new(RetailCorrector.Importers.Taxcom.Instance),
        
        #if DEBUG
        new(RetailCorrector.Importers.Mock.Instance),
        #endif
    ];
}