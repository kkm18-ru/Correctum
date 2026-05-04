using System.Reflection;
using System.Windows;

namespace Correctum;

public partial class App : Application
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public App()
    {
        Utils.Configurations.LogSection.ReloadLoggingConfiguration(false);
        AppContext.SetSwitch("System.Net.DisableIPv6", true);

        var info = Assembly.GetExecutingAssembly().GetName();
        Logger.Info("Запуск {Name} {Version}", info.Name, info.Version);

        InitializeComponent();

        MainWindow = UI.Windows.MainWindow.Instance;
        MainWindow.Show();
    }
}
