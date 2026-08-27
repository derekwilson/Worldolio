using NodaTime;
using System.Globalization;
using System.Text;
using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    // try and not let Noda types leak thru public methods

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
        public DateTime GetNow()
        {
            return _systemTimeProvider.Now;
        }

        public DateTime GetUtcNow()
        {
            return _systemTimeProvider.GetUtcNow();
        }

        private Instant GetInstant(DateTime localDateTime)
        {
            LocalDateTime nodaLocal = LocalDateTime.FromDateTime(localDateTime);
            DateTimeZone zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            return zone.AtLeniently(nodaLocal).ToInstant();
        }

        private Instant GetInstant(int year, int month, int day, int hour, int minute, int second)
        {
            DateTime dt = new DateTime(year, month, day, hour, minute, second);
            DateTime dtUtc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return Instant.FromDateTimeUtc(dtUtc);
        }

        public int GetUtcOffsetSeconds(DateTime localDateTime)
        {
            var instant = GetInstant(localDateTime);
            if (_zone == null)
            {
                throw new InvalidOperationException("Invalid timezone");
            }
            return _zone.GetUtcOffset(instant).Seconds;
        }

        private ZonedDateTime GetLocalTime()
        {
            var instant = GetInstant(_systemTimeProvider.Now);
            return GetLocalTime(instant);
        }

        private ZonedDateTime GetLocalTime(Instant instant)
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

        public string GetFormattedLocalTime(DateTime localTime, TimeFormat format)
        {
            var instant = GetInstant(localTime);
            if (_zone == null)
            {
                return "Unknown";
            }
            return FormatTime(format, GetLocalTime(instant).LocalDateTime);
        }

        public double GetOffsetSeconds(TimeZone otherTz)
        {
            return GetOffsetSeconds(_systemTimeProvider.Now, otherTz);
        }

        public double GetOffsetSeconds(DateTime localDateTime, TimeZone otherTz)
        {
            var instant = GetInstant(localDateTime);
            if (_zone == null || !otherTz.IsValid)
            {
                throw new InvalidOperationException("Invalid timezone");
            }
            Duration myOffset = Duration.FromSeconds(_zone.GetUtcOffset(instant).Seconds);
            Duration otherOffset = Duration.FromSeconds(otherTz.GetUtcOffsetSeconds(localDateTime));

            // we need a Duration as the combined offset may be bigger than 18 Hours which is the maximum allowed in an Offset
            return myOffset.Minus(otherOffset).TotalSeconds;
        }

        public string GetFormattedOffset(TimeZone otherTz)
        {
            return GetFormattedOffset(_systemTimeProvider.Now, otherTz);
        }

        public string GetFormattedOffset(DateTime localTime, TimeZone otherTz)
        {
            var instant = GetInstant(localTime);
            if (_zone == null || !otherTz.IsValid)
            {
                return "Unknown";
            }

            double seconds = GetOffsetSeconds(localTime, otherTz);
            if (seconds == 0)
            {
                return "No offset";
            }

            var offsetStr = $"{(int) (seconds / 3600)}:{(int)(seconds % 3600)}";
            var offsetSuffix = seconds > 0 ? "ahead" : "behind";
            return $"{offsetStr} {offsetSuffix}";
        }

        public string GetDSTDatesForDisplay()
        {
            return GetDSTDatesForDisplay(_systemTimeProvider.Now);
        }

        public string GetDSTDatesForDisplay(DateTime localTime)
        {
            var start = GetInstant(localTime);
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

        private static string FormatTime(TimeFormat format, LocalDateTime time)
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

        public string ToLocalTimeFormatted(DateTime utctime, TimeFormat format)
        {
            if (_zone == null)
            {
                return "Unknown";
            }
            Instant instant = Instant.FromDateTimeUtc(utctime);
            var localtime = instant.InZone(_zone);
            return FormatTime(format, localtime.LocalDateTime);
        }
    }
}
