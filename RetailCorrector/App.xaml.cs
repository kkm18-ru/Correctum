using Dapper;
using RetailCorrector.Utils.Mapper;
using System.Reflection;
using System.Windows;

namespace RetailCorrector
{
    public partial class App : Application
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            SqlMapper.AddTypeHandler(new CurrencyTypeHandler());
            SqlMapper.AddTypeHandler(new FiscalSignTypeHandler());

            Utils.Configurations.LogSection.ReloadLoggingConfiguration(false);
            AppContext.SetSwitch("System.Net.DisableIPv6", true);

            var info = Assembly.GetExecutingAssembly().GetName();
            Logger.Info("Запуск {Name} {Version}", info.Name, info.Version);
        }
    }
}
