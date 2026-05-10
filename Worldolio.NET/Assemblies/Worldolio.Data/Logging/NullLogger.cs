
namespace Worldolio.Data.Logging
{
    /// <summary>
    /// An implementation of ILogger that does nothing
    /// </summary>
    public class NullLogger : ILogger
    {
        public void Debug(ILogger.MessageGenerator message)
        {
        }

        public void Info(ILogger.MessageGenerator message)
        {
        }

        public void LogException(ILogger.MessageGenerator message, Exception ex)
        {
        }

        public void Verbose(ILogger.MessageGenerator message)
        {
        }

        public void Warning(ILogger.MessageGenerator message)
        {
        }
    }
}
