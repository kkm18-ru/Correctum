using System.Reflection;
using System.Windows.Controls;
using RetailCorrector.Actions;
using RetailCorrector.Attributes;
using RetailCorrector.Exporters;
using RetailCorrector.Importers;
using RetailCorrector.UI.Windows;
using RetailCorrector.Utils;

namespace RetailCorrector.UI.Controls;

internal class ImportMenuItem : MenuItem
{
    public ImportMenuItem(Type type)
    {
        Header = type.GetCustomAttribute<ImporterNameAttribute>()?.Name ?? "Безымянный";
        Command = new RelayCommand(_ =>
        {
            var form = (ImporterBase)Activator.CreateInstance(type)!;
            var window = new ImportWindow(form);
            window.ShowDialog();
        });
    }
}

internal class ExportMenuItem : MenuItem
{
    public ExportMenuItem(IExporter exporter)
    {
        Header = exporter.Name;
        Command = new RelayCommand(_ => new ExportWindow(exporter).Show());
    }
}

internal class ActionMenuItem : MenuItem
{
    public ActionMenuItem(IAction action)
    {
        Header = action.Name;
        Command = new RelayCommand(_ => action.Show());
    }
}
