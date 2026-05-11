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
            TIME_SHORT_AMPM = 0,
            TIME_SHORT_24 = 1,
            DATE_LONG = 2,
            DAY_SHORT = 3,
            DATE_TIME_LONG = 4,
            DAY_TIME_SHORT_AMPM = 5,
            DAY_TIME_SHORT_24 = 6,
        }

        /// <summary>
        /// Inflate the enumerated type from supplied int
        /// </summary>
        public static TimeFormat LoadFromInt(int val)
        {
            return (TimeFormat)Enum.ToObject(typeof(TimeFormat), (object)val);
        }

        private readonly DateTimeZone? _zone;
        private ISystemTimeProvider _systemTimeProvider;

        public TimeZone(string ianaId, ISystemTimeProvider systemTimeProvider)
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
            _systemTimeProvider = systemTimeProvider;
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
        public ZonedDateTime GetUtcNow()
        {
            return _systemTimeProvider.GetUtcNow();
        }

        public Offset GetUtcOffset(Instant instant)
        {
            if (_zone == null)
            {
                throw new InvalidOperationException("Invalid timezone");
            }
            return _zone.GetUtcOffset(instant);
        }

        public ZonedDateTime GetLocalTime()
        {
            return GetLocalTime(_systemTimeProvider.Now);
        }

        public ZonedDateTime GetLocalTime(Instant instant)
        {
            if (_zone == null)
            {
                throw new InvalidOperationException("Invalid timezone");
            }
            return instant.InZone(_zone);
        }

        public string GetFormattedLocalTime(TimeFormat format)
        {
            if (_zone == null)
            {
                return "Unknown";
            }
            return FormatTime(format, GetLocalTime().LocalDateTime);
        }

        public string GetFormattedLocalTime(Instant instant, TimeFormat format)
        {
            if (_zone == null)
            {
                return "Unknown";
            }
            return FormatTime(format, GetLocalTime(instant).LocalDateTime);
        }

        public Duration GetOffset(TimeZone otherTz)
        {
            return GetOffset(_systemTimeProvider.Now, otherTz);
        }

        public Duration GetOffset(Instant instant, TimeZone otherTz)
        {
            if (_zone == null || !otherTz.IsValid)
            {
                throw new InvalidOperationException("Invalid timezone");
            }
            Duration myOffset = Duration.FromSeconds(_zone.GetUtcOffset(instant).Seconds);
            Duration otherOffset = Duration.FromSeconds(otherTz.GetUtcOffset(instant).Seconds);

            // we need a Duration as the combined offset may be bigger than 18 Hours which is the maximum allowed in an Offset
            return myOffset.Minus(otherOffset);
        }

        public string GetFormattedOffset(TimeZone otherTz)
        {
            return GetFormattedOffset(_systemTimeProvider.Now, otherTz);
        }

        public string GetFormattedOffset(Instant instant, TimeZone otherTz)
        {
            if (_zone == null || !otherTz.IsValid)
            {
                return "Unknown";
            }

            Duration difference = GetOffset(instant, otherTz);
            if (difference.TotalSeconds == 0)
            {
                return "No offset";
            }

            var offsetStr = difference.ToString("H:mm", CultureInfo.CurrentCulture);
            var offsetSuffix = difference.TotalSeconds > 0 ? "ahead" : "behind";
            return $"{offsetStr} {offsetSuffix}";
        }

        public string GetDSTDatesForDisplay()
        {
            return GetDSTDatesForDisplay(_systemTimeProvider.Now);
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
                dates.Add(FormatTime(TimeFormat.DATE_LONG,interval));
            }
            return string.Join(',',dates);
        }

        private bool IsBetween(DateTimeZone zone, LocalDateTime time, Instant start, Instant end)
        {
            Instant targetInstant = time.InZoneLeniently(zone).ToInstant();
            return targetInstant >= start && targetInstant < end;
        }

        private string FormatTime(TimeFormat format, LocalDateTime time)
        {
            string strTimeFormat = "h:mm tt";
            switch (format)
            {
                case TimeFormat.TIME_SHORT_AMPM:
                    strTimeFormat = "h:mm tt";
                    break;
                case TimeFormat.TIME_SHORT_24:
                    strTimeFormat = "HH:mm";
                    break;
                case TimeFormat.DAY_SHORT:
                    strTimeFormat = "ddd";
                    break;
                case TimeFormat.DATE_LONG:
                    // TODO - take account of the device culture
                    strTimeFormat = "dd MMM yyyy";
                    break;
                case TimeFormat.DATE_TIME_LONG:
                    strTimeFormat = "yyyy MMM dd HH:mm";
                    break;
                case TimeFormat.DAY_TIME_SHORT_AMPM:
                    strTimeFormat = "ddd dd, h:mm tt";
                    break;
                case TimeFormat.DAY_TIME_SHORT_24:
                    strTimeFormat = "ddd dd, HH:mm";
                    break;
            }
            return time.ToString(strTimeFormat, CultureInfo.CurrentCulture);
        }

        public string ToLocalTimeFormatted(ZonedDateTime time, TimeFormat format)
        {
            if (_zone == null)
            {
                return "Unknown";
            }
            var localtime = time.ToInstant().InZone(_zone);
            return FormatTime(format, localtime.LocalDateTime);
        }
    }
}
