using NLog;

namespace Worldolio.Data.Logging
{
    /// <summary>
    /// An implementation of the ILoggerFactory that generates an NlogLogger
    /// </summary>
    public class NLoggerLoggerFactory : ILoggerFactory
    {
        public NLoggerLoggerFactory()
        {
            // set the default loglevel
#if DEBUG
            SetLoggingLevel(LogLevel.Trace);
#else
            SetLoggingLevel(LogLevel.Trace);
#endif
        }

        public void SetLoggingLevel(LogLevel minLevel)
        {
            if (minLevel == LogLevel.Off)
            {
                LogManager.SuspendLogging();
                return;
            }

            if (!LogManager.IsLoggingEnabled())
            {
                LogManager.ResumeLogging();
            }
            if (LogManager.Configuration != null)
            {
                foreach (var rule in LogManager.Configuration.LoggingRules)
                {
                    rule.SetLoggingLevels(minLevel, LogLevel.Fatal);
                }
            }
            // re-apply the config
            LogManager.ReconfigExistingLoggers();
        }

        public ILogger Logger
        {
            get
            {
                var logger = LogManager.GetCurrentClassLogger();
                return new NLogLogger(logger);
            }
        }
    }
}

