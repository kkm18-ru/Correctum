using NLog;
using NLog.Layouts;
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

            LogManager.Configuration = config;
            if (!notify) return;
            stopwatch.Stop();
            LogManager.GetCurrentClassLogger().Info(
                "Конфигурация журнала обновлена за {Duration} мс.", 
                stopwatch.ElapsedMilliseconds);
        }
    }
}
