using System.Windows.Controls;
using Microsoft.Xaml.Behaviors.Core;
using Correctum.Importers;

namespace Correctum.UI.Controls;

internal class ImportMenuItem : MenuItem
{
    public ImportMenuItem(IImporter importer)
    {
        Header = importer.Name;
        Command = new ActionCommand(importer.Show);
    }
}
