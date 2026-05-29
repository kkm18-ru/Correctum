using NLog;
using NLog.Targets;
using NLog.Targets.Seq;
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

            var logFile = new FileTarget("log_file")
            {
                ArchiveEvery = FileArchivePeriod.Monday,
                FileName = "${basedir}/logs/${date:format=yyyyMMdd}.log",
                Layout = "${longdate} [${level:uppercase=true:format=TriLetter} ${logger:shortName=true}] ${message}${newline:when=exception!=null}${exception}"
            };
            config.AddRule(LogLevel.FromOrdinal(conf.Local.MinLevel), LogLevel.Fatal, logFile);

            if(!string.IsNullOrWhiteSpace(conf.Seq.Endpoint) && !string.IsNullOrWhiteSpace(conf.Seq.Key))
            {
                var logSeq = new SeqTarget
                {
                    Name = "log_seq",
                    ApiKey = conf.Seq.Key,
                    ServerUrl = conf.Seq.Endpoint
                };
                logSeq.Properties.Add(new SeqPropertyItem { Name = "Application", Value = "Correctum" });
                logSeq.Properties.Add(new SeqPropertyItem { Name = "MachineName", Value = "${machinename}" });
                logSeq.Properties.Add(new SeqPropertyItem { Name = "ProgressId", Value = "${processid}" });
                logSeq.Properties.Add(new SeqPropertyItem { Name = "MethodName", Value = "${callsite}" });
                config.AddRule(LogLevel.FromOrdinal(conf.Seq.MinLevel), LogLevel.Fatal, logSeq);
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
