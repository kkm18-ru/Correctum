using Correctum.Abstractions;
using Correctum.Converters.Toml;
using PuppeteerSharp;
using System.IO;
using Tomlyn;
using Tomlyn.Serialization;

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
        try
        {
            using var fs = File.OpenRead(FileName);
            Current = TomlSerializer.Deserialize(fs, ConfigurationContext.Default.Configuration)!;
            Logger.Info("Файл конфигурации загружен!");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Не удалось загрузить конфигурацию.");
            Current = new Configuration();
        }
    }

    public void Save()
    {
        File.WriteAllText(FileName, TomlSerializer.Serialize(this, ConfigurationContext.Default.Configuration));
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
        public LocalLoggingConfigurationSection Local { get; } = new();
        public SeqLoggingConfigurationSection Seq { get; } = new();

        public class LocalLoggingConfigurationSection
        {
            public NLog.LogLevel MinLevel { get; set; } = NLog.LogLevel.Info;
        }

        public class SeqLoggingConfigurationSection
        {
            public NLog.LogLevel MinLevel { get; set; } = NLog.LogLevel.Info;
            public string Endpoint { get; set; } = string.Empty;
            public string Key { get; set; } = string.Empty;
        }
    }
}

[TomlSerializable(typeof(Configuration))]
[TomlSourceGenerationOptions(Converters = [typeof(LogLevelTomlConverter)])]
public sealed partial class ConfigurationContext : TomlSerializerContext;