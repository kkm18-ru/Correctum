using System.Windows.Controls;
using Microsoft.Xaml.Behaviors.Core;
using RetailCorrector.Importers;

namespace RetailCorrector.UI.Controls;

internal class ImportMenuItem : MenuItem
{
    public ImportMenuItem(IImporter importer)
    {
        Header = importer.Name;
        Command = new ActionCommand(importer.Show);
    }
}
