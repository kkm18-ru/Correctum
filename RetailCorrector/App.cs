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

