using RetailCorrector.Attributes;
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
        Resources.Add("jbmono", new FontFamily(new(Path.Combine(dir, "JetBrainsMono.ttf")), "#JetBrains Mono"));
    }
}

