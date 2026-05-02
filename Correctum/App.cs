using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Correctum;

public partial class App : Application
{
    public static App Instance => field ??= new App();
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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
        Logger.Info("Загружен шрифт {FamilyName}", familyName);
    }
}

