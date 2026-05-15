using NLog;

namespace Worldolio.Data.Logging
{
    /// <summary>
    /// An implementation of the ILoggerFactory that generates an NlogLogger
    /// </summary>
    public class NLogLoggerFactory : ILoggerFactory
    {
        public NLogLoggerFactory()
        {
            // set the default loglevel
#if DEBUG
            NLogHelper.SetLoggingLevel(LogLevel.Trace);
#else
            NLogHelper.SetLoggingLevel(LogLevel.Trace);
#endif
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

