using RetailCorrector.Attributes;
using Serilog;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace RetailCorrector;

[Singleton]
public partial class App : Application
{
    private App()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        LoadFonts();
        ConfigureLogger();
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

    private void LoadFonts()
    {
        var dir = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts");
        Resources.Add("jbmono", new FontFamily(new(Path.Combine(dir, "JetBrainsMono.ttf")), "#JetBrains Mono"));
    }
}

