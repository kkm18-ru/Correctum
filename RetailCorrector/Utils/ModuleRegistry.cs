using System.Collections.Immutable;
using RetailCorrector.UI.Controls;

namespace RetailCorrector.Utils;

internal static class ModuleRegistry
{
    public static readonly ImmutableArray<Parsers.IParser> Parsers = [
        RetailCorrector.Parsers.PlatformOfd.Instance,
        RetailCorrector.Parsers.OfdRuJson.Instance,
    ];

    public static readonly ImmutableArray<ExportMenuItem> Exporters = [
        new(RetailCorrector.Exporters.Fptr.Instance),
        new(RetailCorrector.Exporters.Sqlite.Instance),
    ];

    public static readonly ImmutableArray<ActionMenuItem> Actions = [
        new(RetailCorrector.Actions.CancelReceipt.Instance),
        new(RetailCorrector.Actions.RemoveReceipt.Instance),
    ];

    public static readonly ImmutableArray<ImportMenuItem> Importers = [
        new(RetailCorrector.Importers.Saby.Instance),
        new(RetailCorrector.Importers.Sqlite.Instance),
        new(RetailCorrector.Importers.OfdRu.Instance),
        new(RetailCorrector.Importers.PlatformOfd.Instance),
        new(RetailCorrector.Importers.FxPOS.Instance),
        new(RetailCorrector.Importers.Acquirer.Instance),
        
        #if DEBUG
        new(RetailCorrector.Importers.Mock.Instance),
        #endif
    ];
}