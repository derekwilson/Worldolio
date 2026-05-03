using NodaTime;

namespace Worldolio.Data.Model
{
    public class TimeZone
    {
        private DateTimeZone? _zone;

        public TimeZone(string ianaId)
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
    }
}
