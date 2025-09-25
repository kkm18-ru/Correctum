using System.Windows;

[assembly: ThemeInfo(ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly)]

namespace RetailCorrector
{
    internal class Program
    {
        [STAThread]
        public static void Main()
        {
            App.Instance.Run(UI.Windows.MainWindow.Instance);
        }
    }
}
