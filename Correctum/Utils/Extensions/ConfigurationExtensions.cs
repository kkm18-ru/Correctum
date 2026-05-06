using NLog;
using NLog.Layouts;
using NLog.Loki;
using NLog.Targets;
using System.Diagnostics;

namespace Correctum.Utils.Extensions;

public static class ConfigurationExtensions
{
    extension(Configuration.LoggingConfigurationSection conf)
    {
        public void Apply(bool notify)
        {
            var stopwatch = new Stopwatch();
            if (notify) stopwatch.Start();
            var config = new NLog.Config.LoggingConfiguration();
            var minLevel = LogLevel.FromOrdinal(conf.MinLevel);

            var logFile = new FileTarget("log_file")
            {
                ArchiveEvery = FileArchivePeriod.Monday,
                FileName = conf.FilePath,
                Layout = Layout.FromString("${longdate} [${level:uppercase=true:format=TriLetter} ${logger:shortName=true}] ${message}${newline:when=exception!=null}${exception}")
            };
            config.AddRule(minLevel, LogLevel.Fatal, logFile);

            if (conf.LokiEnabled)
            {
                var logLoki = new LokiTarget
                {
                    Name = "log_loki",
                    Endpoint = new SimpleLayout(conf.LokiEndpoint),
                    Labels = {
                    new LokiTargetLabel { Name = "app", Layout = "RetailCorrector" },
                    new LokiTargetLabel { Name = "host", Layout = conf.LokiHostName },
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
                if (conf.LokiAuthEnabled)
                {
                    logLoki.Username = conf.LokiUsername;
                    logLoki.Password = conf.LokiPassword;
                }
                config.AddRule(minLevel, LogLevel.Fatal, logLoki);
            }

            LogManager.Configuration = config;
            if (!notify) return;
            stopwatch.Stop();
            LogManager.GetCurrentClassLogger().Info(
                "Конфигурация журнала обновлена за {Duration} мс.", 
                stopwatch.ElapsedMilliseconds);
        }
    }
}
