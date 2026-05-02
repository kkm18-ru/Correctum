using Tomlyn;
using Correctum.Utils.Configurations;
using System.IO;

namespace Correctum.Utils
{
    internal partial class Configuration
    {
        public static Configuration Instance
        {
            get
            {
                if (field is not null) return field;
                if (!File.Exists(FileName))
                {
                    Logger.Warn("Файл конфигурации отсутствует!");
                    return field = new Configuration();
                }
                if (!TomlSerializer.TryDeserialize<Configuration>(File.ReadAllText(FileName), out var conf))
                {
                    Logger.Error("Не удалось выполнить загрузку конфигурации.");
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
            File.WriteAllText(FileName, TomlSerializer.Serialize(this));
            Logger.Info("Конфигурация сохранена!");
        }
    }
}
