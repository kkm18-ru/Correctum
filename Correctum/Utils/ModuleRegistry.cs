using System.Collections.Immutable;
using Correctum.UI.Controls;

namespace Correctum.Utils;

internal static class ModuleRegistry
{
    public static readonly ImmutableArray<Parsers.IParser> Parsers = [
        Correctum.Parsers.PlatformOfd.Instance,
        Correctum.Parsers.OfdRuJson.Instance,
    ];

    public static readonly ImmutableArray<ImportMenuItem> Importers = [
        new(Correctum.Importers.Saby.Instance),
        new(Correctum.Importers.Sqlite.Instance),
        new(Correctum.Importers.OfdRu.Instance),
        new(Correctum.Importers.PlatformOfd.Instance),
        new(Correctum.Importers.Acquirer.Instance),
        new(Correctum.Importers.Taxcom.Instance),
        new(Correctum.Importers.FirstOfd.Instance),
        
        #if DEBUG
        new(Correctum.Importers.Mock.Instance),
        #endif
    ];
}