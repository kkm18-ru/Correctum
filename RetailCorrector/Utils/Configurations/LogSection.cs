namespace RetailCorrector.Utils.Configurations;

internal sealed class LogSection
{
    public string FilePath { get; set; } = "./logs/current.log";
    public bool LokiEnabled { get; set; }
    public string LokiEndpoint { get; set; } = "http://localhost:3100";
    public string LokiHostName { get; set; } = "MainHost";
    public bool LokiAuthEnabled { get; set; }
    public string LokiUsername { get; set; } = "admin";
    public string LokiPassword { get; set; } = "1234567890";
}