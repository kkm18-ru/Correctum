using Correctum.Fptr;
using Correctum.Utils;
using System.Reflection;
using System.Windows;

namespace Correctum;

public partial class App : Application
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public App()
    {
        FptrLibrary.Load();
        Configuration.Load();
        Configuration.Current.Logging.Apply(false);
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            Logger.Fatal((Exception)e.ExceptionObject, "Необработанная ошибка.");
        AppContext.SetSwitch("System.Net.DisableIPv6", true);

        var info = Assembly.GetExecutingAssembly().GetName();
        Logger.Info("Запуск {Name} {Version}", info.Name, info.Version!.ToString(3));

        InitializeComponent();

        MainWindow = UI.Windows.MainWindow.Instance;
        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        FptrLibrary.Free();
        base.OnExit(e);
    }
}
