using System.Collections.Immutable;
using Correctum.UI.Controls;
using Correctum.Views.Windows;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Correctum.Utils;

internal static partial class ModuleRegistry
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

public readonly record struct ReportDescriptor(string DisplayName, Func<DataTable> Function);

public static class ModuleDescriptorHelper
{
    public static MenuItem ToMenuItem(ReportDescriptor descriptor)
    {
        var menuItem = new MenuItem()
        {
            Header = descriptor.DisplayName,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center
        };

        menuItem.Click += (_, _) => 
            new ReportWindow(descriptor.DisplayName, descriptor.Function).ShowDialog();

        return menuItem;
    }
}
