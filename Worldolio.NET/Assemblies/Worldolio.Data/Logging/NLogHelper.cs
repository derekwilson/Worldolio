using NLog;

namespace Worldolio.Data.Logging
{
    public static class NLogHelper
    {
        public static void SetLoggingLevel(LogLevel minLevel)
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
    }
}
