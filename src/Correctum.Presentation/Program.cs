using Avalonia;
using Correctum.Extensions;
using NLog;

namespace Correctum
{
    internal sealed class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Configurations.Configuration.Initialize();
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont().LogToTrace()
                .StartWithClassicDesktopLifetime(args);
        }
    }
}
