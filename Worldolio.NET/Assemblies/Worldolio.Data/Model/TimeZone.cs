using NodaTime;
using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    public class TimeZone
    {
        private DateTimeZone? _zone;
        private IInstantProvider _instantProvider;

        public TimeZone(string ianaId, IInstantProvider instantProvider)
        {
            var tzdb = DateTimeZoneProviders.Tzdb;
            try
            {
                _zone = tzdb[ianaId];
            }
            catch 
            {
                _zone = null;
            }
            _instantProvider = instantProvider;
        }

        public bool IsValid
        {
            get
            {
                return _zone != null;
            }
        }

        public string GetDisplayName()
        {
            if (IsValid)
            {
                return _zone?.ToString() ?? "NULL";
            }
            return "Unknown";
        }

        public string GetNow()
        {
            if (!IsValid)
            {
                return "Unknown";
            }
            ZonedDateTime time = _instantProvider.Now.InZone(_zone);
            return time.ToString("F", System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
