using NodaTime;
using System.Globalization;
using System.Text;
using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    public class TimeZone
    {
        public enum TimeFormat
        {
            SHORT_AMPM = 0,
            SHORT_24 = 1
        }

        /// <summary>
        /// Inflate the enumerated type from supplied int
        /// </summary>
        public static TimeFormat LoadFromInt(int val)
        {
            return (TimeFormat)Enum.ToObject(typeof(TimeFormat), (object)val);
        }

        private readonly DateTimeZone? _zone;
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
            if (_zone == null)
            {
                return "Unknown";
            }
            return _zone.ToString();
        }

        public ZonedDateTime GetLocalTime()
        {
            return GetLocalTime(_instantProvider.Now);
        }

        public ZonedDateTime GetLocalTime(Instant instant)
        {
            if (_zone == null)
            {
                throw new InvalidOperationException("Invalid timezone");
            }
            return instant.InZone(_zone);
        }

        public string GetFormattedLocalTime()
        {
            if (_zone == null)
            {
                return "Unknown";
            }
            return FormatTime(TimeFormat.SHORT_AMPM, GetLocalTime());
        }

        public string GetFormattedLocalTime(Instant instant)
        {
            if (_zone == null)
            {
                return "Unknown";
            }
            return FormatTime(TimeFormat.SHORT_AMPM, GetLocalTime(instant));
        }

        public string GetDSTDatesForDisplay()
        {
            return GetDSTDatesForDisplay(_instantProvider.Now);
        }

        public string GetDSTDatesForDisplay(Instant start)
        {
            if (_zone == null)
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
                .Where(intervalEnd => IsBetween(_zone, intervalEnd, start, end));

            List<string> dates = [];
            foreach (var interval in intervals)
            {
                dates.Add(interval.ToString("dd MMM yyyy", CultureInfo.CurrentCulture));
            }
            return string.Join(',',dates);
        }

        private bool IsBetween(DateTimeZone zone, LocalDateTime time, Instant start, Instant end)
        {
            Instant targetInstant = time.InZoneLeniently(zone).ToInstant();
            return targetInstant >= start && targetInstant < end;
        }

        private string FormatTime(TimeFormat format, ZonedDateTime time)
        {
            string strTimeFormat = "h:mm tt";
            switch (format)
            {
                case TimeFormat.SHORT_AMPM:
                    strTimeFormat = "h:mm tt";
                    break;
                case TimeFormat.SHORT_24:
                    strTimeFormat = "HH:mm";
                    break;
            }
            return time.ToString(strTimeFormat, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
