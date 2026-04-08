using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

[assembly: InternalsVisibleTo("RetailCorrector.Tests")]
[assembly: ThemeInfo(ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly)]

namespace RetailCorrector
{
    static class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        [STAThread]
        public static void Main()
        {
            Utils.Configurations.LogSection.ReloadLoggingConfiguration(false);
            AppContext.SetSwitch("System.Net.DisableIPv6", true);

            var info = Assembly.GetExecutingAssembly().GetName();
            Logger.Info("Запуск {Name} {Version}", info.Name, info.Version);

            App.Instance.Run(UI.Windows.MainWindow.Instance);
        }
    }
}
