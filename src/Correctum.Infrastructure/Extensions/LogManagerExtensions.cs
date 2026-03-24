using System.Diagnostics;
using Correctum.Configurations;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Loki;
using NLog.Targets;

namespace Correctum.Extensions;

public static class LogManagerExtensions
{
    extension(LogManager)
    {
        public static void InitializeConfiguration()
        {
            var config = new LoggingConfiguration();
            var minLevel = LogLevel.FromOrdinal((int)Configuration.Current.Logging.MinLevel);
            
            var logFile = new FileTarget("log_file")
            {
                ArchiveEvery = FileArchivePeriod.Monday,
                FileName = Configuration.Current.Logging.FilePath
            };
            config.AddRule(minLevel, LogLevel.Fatal, logFile);

            if (Configuration.Current.Logging.Loki.Enabled)
            {
                var logLoki = new LokiTarget
                {
                    Name = "log_loki", 
                    Endpoint = new SimpleLayout(Configuration.Current.Logging.Loki.Endpoint),
                    Labels = {
                        new LokiTargetLabel { Name = "app", Layout = "Correctum" }, 
                        new LokiTargetLabel { Name = "host", Layout = Configuration.Current.Logging.Loki.HostName }, 
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
                if (Configuration.Current.Logging.Loki.AuthEnabled)
                {
                    logLoki.Username = Configuration.Current.Logging.Loki.Username;
                    logLoki.Password = Configuration.Current.Logging.Loki.Password;
                }
                config.AddRule(minLevel, LogLevel.Fatal, logLoki);
            }
            
            LogManager.Configuration = config;
        }
        public static void ReloadConfiguration()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            InitializeConfiguration();
            stopwatch.Stop();
            LogManager.GetLogger("LoggingConfiguration")
                .Info("Конфигурация логирования перезагружена за {Duration} мс.", 
                    stopwatch.ElapsedMilliseconds);
        }
    }
}