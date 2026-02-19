using System.Diagnostics;
using System.Text;
using NLog;
using NLog.Layouts;
using NLog.Loki;
using NLog.Targets;

namespace RetailCorrector.Utils.Configurations;

internal sealed class LogSection
{
    public string FilePath { get; set; } = "./logs/current.log";
    public int MinLevel { get; set; } = LogLevel.Info.Ordinal;
    public bool LokiEnabled { get; set; }
    public string LokiEndpoint { get; set; } = "http://localhost:3100";
    public string LokiHostName { get; set; } = "MainHost";
    public bool LokiAuthEnabled { get; set; }
    public string LokiUsername { get; set; } = "admin";
    public string LokiPassword
    {
        get => field.StartsWith("enc\0") ? SecretHelper.DecodeSecret(field[4..], Encoding.Unicode) : field;
        set => field = $"enc\0{SecretHelper.EncodeSecret(value, Encoding.Unicode)}";
    } = string.Empty;

    public static void ReloadLoggingConfiguration(bool notify)
    {
        var stopwatch = new Stopwatch();
        if (notify) stopwatch.Start();
        var config = new NLog.Config.LoggingConfiguration();
        var minLevel = LogLevel.FromOrdinal(Configuration.Instance.Logging.MinLevel);

        var logFile = new FileTarget("log_file")
        {
            ArchiveEvery = FileArchivePeriod.Monday,
            FileName = Configuration.Instance.Logging.FilePath
        };
        config.AddRule(minLevel, LogLevel.Fatal, logFile);

        if (Configuration.Instance.Logging.LokiEnabled)
        {
            var logLoki = new LokiTarget
            {
                Name = "log_loki", 
                Endpoint = new SimpleLayout(Configuration.Instance.Logging.LokiEndpoint),
                Labels = {
                    new LokiTargetLabel { Name = "app", Layout = "RetailCorrector" }, 
                    new LokiTargetLabel { Name = "host", Layout = Configuration.Instance.Logging.LokiHostName }, 
                },
                Layout = new JsonLayout
                {
                    IncludeEventProperties = true,
                    Attributes =
                    {
                        new JsonAttribute("level", "${level}"),
                        new JsonAttribute("source", "${logger}"),
                        new JsonAttribute("message", "${message}"),
                        new JsonAttribute("exception", new JsonLayout
                        {
                            Attributes =
                            {
                                new JsonAttribute("type", "${exception:format=type}"),
                                new JsonAttribute("message", "${exception:format=message}"),
                                new JsonAttribute("stacktrace", "${exception:format=tostring}")
                            }
                        }) { Encode = false },
                    }
                }
            };
            if (Configuration.Instance.Logging.LokiAuthEnabled)
            {
                logLoki.Username = Configuration.Instance.Logging.LokiUsername;
                logLoki.Password = Configuration.Instance.Logging.LokiPassword;
            }
            config.AddRule(minLevel, LogLevel.Fatal, logLoki);
        }
        
        LogManager.Configuration = config;
        if (!notify) return;
        stopwatch.Stop();
        LogManager.GetCurrentClassLogger()
            .Info("Конфигурация журнала обновлена за {Duration} мс.", stopwatch.ElapsedMilliseconds);
    }
}