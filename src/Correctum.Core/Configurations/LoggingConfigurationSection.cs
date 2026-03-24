using Correctum.Enums;

namespace Correctum.Configurations;

public sealed class LoggingConfigurationSection
{
    public string FilePath { get; set; }
    public LogLevel MinLevel { get; set; }
    public LokiConfigurationSection Loki { get; set; }

    public static LoggingConfigurationSection Default => new()
    {
        FilePath = "logs/latest.log",
        MinLevel = LogLevel.Info,
        Loki = LokiConfigurationSection.Default
    };
}