using Tomlyn;
using RetailCorrector.Utils.Configurations;
using System.IO;

namespace RetailCorrector.Utils
{
    internal partial class Configuration
    {
        public static Configuration Instance
        {
            get
            {
                if(field is not null) return field;
                if (!File.Exists(FileName))
                {
                    Logger.Warn("Файл конфигурации отсутствует!");
                    return field = new Configuration(); 
                }
                if (!Toml.TryToModel<Configuration>(File.ReadAllText(FileName), out var conf, out var errors))
                {
                    Logger.Error("Не удалось выполнить загрузку конфигурации: {Errors}", errors);
                    return field = new Configuration(); 
                }
                Logger.Info("Файл конфигурации загружен!");
                return field = conf;
            }
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const string FileName = "config.toml";

        public SqliteSection Sqlite { get; init; } = new();
        public FptrSection Fptr { get; init; } = new();
        public HttpClientSection HttpClient { get; init; } = new();
        public ExportersSection Exporters { get; init; } = new();
        public ImportersSection Importers { get; init; } = new();
        public LogSection Logging { get; init; } = new();

        public void Save()
        {
            File.WriteAllText(FileName, Toml.FromModel(this));
            Logger.Info("Конфигурация сохранена!");
        }
    }
}
