using System.Windows.Controls;
using RetailCorrector.Actions;
using RetailCorrector.Importers;
using RetailCorrector.Utils;

namespace RetailCorrector.UI.Controls;

internal class ImportMenuItem : MenuItem
{
    public ImportMenuItem(IImporter importer)
    {
        Header = importer.Name;
        Command = new RelayCommand(_ => importer.Show());
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
