using Correctum.Abstractions;
using PuppeteerSharp;
using System.IO;
using System.Text;
using Tomlyn;

namespace Correctum.Utils;

public partial class Configuration
{
    private const string FileName = "config.toml";

    public static Configuration Current { get; private set; } = null!;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public ImportConfigurationSection Import { get; } = new();
    public FptrConfigurationSection Fptr { get; } = new();
    public LoggingConfigurationSection Logging { get; } = new();
    public HttpConfigurationSection Http { get; } = new();

    public static void Load()
    {
        if (!File.Exists(FileName))
        {
            Logger.Warn("Файл конфигурации отсутствует!");
            Current = new Configuration();
            return;
        }
        if (!TomlSerializer.TryDeserialize<Configuration>(File.ReadAllText(FileName), out var conf))
        {
            Logger.Error("Не удалось выполнить загрузку конфигурации.");
            Current = new Configuration();
            return;
        }
        Logger.Info("Файл конфигурации загружен!");
        Current = conf;
    }

    public void Save()
    {
        File.WriteAllText(FileName, TomlSerializer.Serialize(this));
        Logger.Info("Конфигурация сохранена!");
    }

    public class ImportConfigurationSection
    {
        public SqliteImportConfigurationSection Sqlite { get; } = new();
        public TaxcomImportConfigurationSection Taxcom { get; } = new();

        public class SqliteImportConfigurationSection
        {
            public int StackSize { get; set; } = 100;
        }

        public class TaxcomImportConfigurationSection
        {
            public string IntegratorId { get; set; } = "";
        }
    }

    public class HttpConfigurationSection : BindableBase
    {
        public int RequestDelay { get; set; } = 1000;
        public byte TriesCount { get; set; } = 3;
        public short PageSize { get; set; } = 100;
        public string WebBrowserPath { get; set => SetField(ref field, value); } = 
            @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
        public ProtocolType WebBrowserProtocol { get; set; } = ProtocolType.Cdp;
        public SupportedBrowser WebBrowserType { get; set; } = SupportedBrowser.Chromium;
    }

    public class FptrConfigurationSection
    {
        public int SizeStack { get; set; } = 1000;
        public int StackIntervalMinutes { get; set; } = 10;
    }

    public class LoggingConfigurationSection
    {
        public string FilePath { get; set; } = "logs/current.log";
        public int MinLevel { get; set; } = 2; // 2 - уровень информации
    }
}
