using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    public interface ITimeZoneFactory
    {
        TimeZone GetTimeZoneFromIanaName(string name);
    }

    public class TimeZoneFactory : ITimeZoneFactory
    {
        private ISystemTimeProvider _systemTimeProvider;

        public TimeZoneFactory(ISystemTimeProvider systemTimeProvider)
        {
            _systemTimeProvider = systemTimeProvider;
        }

        public TimeZone GetTimeZoneFromIanaName(string name)
        {
            return new TimeZone(name, _systemTimeProvider);
        }
    }
}
