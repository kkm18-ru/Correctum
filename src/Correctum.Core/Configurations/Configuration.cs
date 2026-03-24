namespace Correctum.Configurations;

public sealed class Configuration
{
    public static Configuration Current { get; set; }

    public HttpClientConfigurationSection HttpClient { get; set; }
    public ImportersConfigurationSection Importers { get; set; }
    public LoggingConfigurationSection Logging { get; set; }
}
