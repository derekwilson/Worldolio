using NLog;

namespace Worldolio.Data.Logging
{
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
}
