using NodaTime;
using System.Globalization;
using System.Text;
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

        public string GetDSTDatesForDisplay()
        {
            return GetDSTDatesForDisplay(_instantProvider.Now);
        }

        public string GetDSTDatesForDisplay(Instant start)
        {
            if (!IsValid)
            {
                return "Unknown";
            }
            Instant end = start
                            .InUtc()
                            .LocalDateTime
                            .PlusYears(1)
                            .InUtc()
                            .ToInstant();
            var allIntervals = _zone.GetZoneIntervals(start, end);

            StringBuilder str = new StringBuilder(100);
            if (allIntervals.Count() == 1)
            {
                return "No DST";
            }

            // if you are getting 2026 to 2027 then allIntervals will contain all the intervals for 2027, we only one the ones in the next year
            var intervals = allIntervals
                .Where(i => i.HasEnd)
                .Select(i => i.IsoLocalEnd)
                .Where(intervalEnd => isBetween(_zone, intervalEnd, start, end));

            foreach (var interval in intervals)
            {
                str.Append(interval.ToString("dd MMM yyyy", CultureInfo.CurrentCulture));
                str.Append(' ');
            }
            return str.ToString();
        }

        private bool isBetween(DateTimeZone zone, LocalDateTime time, Instant start, Instant end)
        {
            Instant targetInstant = time.InZoneLeniently(zone).ToInstant();
            return targetInstant >= start && targetInstant < end;
        }

    }
}
