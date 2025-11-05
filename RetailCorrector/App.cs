using Serilog;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace RetailCorrector;

public partial class App : Application
{
    private static App _instance = null!;
    public static App Instance => _instance ??= new App();

    private App()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        LoadFonts();
    }

    private void LoadFonts()
    {
        var dir = Path.Combine(AppContext.BaseDirectory, "assets", "fonts");
        LoadFont("jbmono", "#JetBrains Mono", Path.Combine(dir, "JetBrainsMono.ttf"));
    }

    private void LoadFont(string id, string familyName, string path)
    {
        Resources.Add(id, new FontFamily(new(path), familyName));
        Log.Information("Загружен шрифт {FamilyName}", familyName);
    }
}

