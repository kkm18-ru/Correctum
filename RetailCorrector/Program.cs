using Serilog;
using System.Windows;

[assembly: ThemeInfo(ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly)]

namespace RetailCorrector
{
    static class Program
    {
        [STAThread]
        public static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: ".\\logs\\.log",
                    rollingInterval: RollingInterval.Day,
                    flushToDiskInterval: TimeSpan.FromMilliseconds(100))
                .CreateLogger();

            App.Instance.Run(UI.Windows.MainWindow.Instance);
        }
    }
}
