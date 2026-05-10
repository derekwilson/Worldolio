namespace Worldolio.Data.Logging
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
}
