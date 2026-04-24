using System.Windows.Controls;
using RetailCorrector.Importers;
using RetailCorrector.Utils;

namespace RetailCorrector.UI.Controls;

internal class ImportMenuItem : MenuItem
{
    public ImportMenuItem(IImporter importer)
    {
        Header = importer.Name;
        Command = new RelayCommand(importer.Show);
    }
}
