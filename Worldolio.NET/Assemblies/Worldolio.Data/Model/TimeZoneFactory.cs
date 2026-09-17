using Worldolio.Data.Logging;

namespace Worldolio.Data.Model
{
    public interface ITimeZoneFactory
    {
        TimeZone GetTimeZoneFromIanaName(string name);
    }

    public class TimeZoneFactory : ITimeZoneFactory
    {
        private ILogger _logger;

        public TimeZoneFactory(ILogger logger)
        {
            _logger = logger;
        }

        public TimeZone GetTimeZoneFromIanaName(string name)
        {
            _logger.Debug(() => $"TimeZoneFactory.GetTimeZoneFromIanaName, {name}");
            return new TimeZone(name);
        }
    }
}
