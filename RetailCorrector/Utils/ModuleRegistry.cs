using System.Collections.Immutable;
using RetailCorrector.Importers;
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

    public static readonly ImmutableArray<ImportMenuItem> Importers = [..SearchImporters()];

    private static IEnumerable<ImportMenuItem> SearchImporters() =>
        from type in typeof(Program).Assembly.GetTypes() 
        where type.BaseType == typeof(ImporterBase) 
        select new ImportMenuItem(type);
}