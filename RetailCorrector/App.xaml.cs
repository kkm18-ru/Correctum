using Serilog;
using System.IO;
using System.Windows;

namespace RetailCorrector;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        ConfigureLogger();
        Workspace.New();
    }

    static void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RetailCorrector.log"),
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss}|{Level}|{LogContext}|{Message}{NewLine}",
                flushToDiskInterval: TimeSpan.FromMilliseconds(100))
            .CreateLogger();
    }
}

