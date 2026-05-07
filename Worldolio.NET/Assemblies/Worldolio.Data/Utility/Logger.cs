using NLog;

namespace Worldolio.Data.Utility
{
    public interface ILogger
    {
        public delegate string MessageGenerator();

        void Verbose(MessageGenerator message);
        void Debug(MessageGenerator message);
        void Info(MessageGenerator message);
        void Warning(MessageGenerator message);
        void LogException(MessageGenerator message, Exception ex);
    }

    /// <summary>
    /// An implementation of ILogger that uses NLog
    /// </summary>
    public class NLogLogger : ILogger
    {
        private Logger nlogLogger;

        public NLogLogger(Logger logger)
        {
            nlogLogger = logger;
        }

        public void Info(ILogger.MessageGenerator message)
        {
            if (nlogLogger.IsEnabled(LogLevel.Info))
            {
                // only call the message delegate if we are logging
                nlogLogger.Info(message());
            }
        }

        public void Debug(ILogger.MessageGenerator message)
        {
            if (nlogLogger.IsEnabled(LogLevel.Debug))
            {
                // only call the message delegate if we are logging
                nlogLogger.Debug(message());
            }
        }

        public void Warning(ILogger.MessageGenerator message)
        {
            if (nlogLogger.IsEnabled(LogLevel.Warn))
            {
                // only call the message delegate if we are logging
                nlogLogger.Warn(message());
            }
        }

        public void LogException(ILogger.MessageGenerator message, Exception ex)
        {
            nlogLogger.Error(ex, message() + $" => {ex.Message}");
        }

        public void Verbose(ILogger.MessageGenerator message)
        {
            if (nlogLogger.IsEnabled(LogLevel.Trace))
            {
                // only call the message delegate if we are logging
                nlogLogger.Trace(message());
            }
        }
    }

    public interface ILoggerFactory
    {
        ILogger Logger { get; }
    }

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
