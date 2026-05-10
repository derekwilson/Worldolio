namespace Worldolio.Data.Logging
{
    /// <summary>
    /// An implementation of the ILoggerFactory that generates an NullLogger
    /// </summary>
    public class NullLoggerFactory : ILoggerFactory
    {
        private ILogger _logger = new NullLogger();

        public ILogger Logger
        {
            get
            {
                return _logger;
            }
        }
    }
}

