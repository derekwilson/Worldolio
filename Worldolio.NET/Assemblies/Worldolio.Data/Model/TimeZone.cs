using NodaTime;
using System.Globalization;
using System.Text;

namespace Worldolio.Data.Model
{
    // try and not let Noda types leak thru the interface
    public interface ITimeZone
    {
        enum TimeFormat
        {
            TIME_SHORT_AMPM = 0,
            TIME_SHORT_24 = 1,
            DATE_LONG = 2,
            DAY_SHORT = 3,
            DATE_TIME_LONG = 4,
            DAY_TIME_SHORT_AMPM = 5,
            DAY_TIME_SHORT_24 = 6,
        }

        bool IsValid { get; }
        string ToLocalTimeFromUtcFormatted(DateTime utctime, ITimeZone.TimeFormat format);
        string GetFormattedLocalTime(DateTime localTime, ITimeZone.TimeFormat format);
        string GetFormattedOffset(DateTime localTime, ITimeZone otherTz);
        string GetDSTDatesForDisplay(DateTime localTime);
        int GetUtcOffsetSeconds(DateTime localDateTime);
    }

    public static class TimeZoneHelper
    {
        /// <summary>
        /// Inflate the enumerated type from supplied int
        /// </summary>
        public static ITimeZone.TimeFormat LoadFromInt(int val)
        {
            return (ITimeZone.TimeFormat)Enum.ToObject(typeof(ITimeZone.TimeFormat), (object)val);
        }
    }


    // try and not let Noda types leak thru public methods

    public class TimeZone : ITimeZone
    {
        private readonly DateTimeZone? _zone;
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
            if (_zone == null)
            {
                return "Unknown";
            }
            return _zone.ToString();
        }

        private Instant GetInstant(DateTime localDateTime)
        {
            LocalDateTime nodaLocal = LocalDateTime.FromDateTime(localDateTime);
            DateTimeZone zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            return zone.AtLeniently(nodaLocal).ToInstant();
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

        private ZonedDateTime GetLocalTime(Instant instant)
        {
            if (_zone == null)
            {
                throw new InvalidOperationException("Invalid timezone");
            }
            return instant.InZone(_zone);
        }

        public string GetFormattedLocalTime(DateTime localTime, ITimeZone.TimeFormat format)
        {
            var instant = GetInstant(localTime);
            if (_zone == null)
            {
                return "Unknown";
            }
            return FormatTime(format, GetLocalTime(instant).LocalDateTime);
        }

        private double GetOffsetSeconds(DateTime localDateTime, ITimeZone otherTz)
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

        public string GetFormattedOffset(DateTime localTime, ITimeZone otherTz)
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
            int absSeconds = (int) Math.Abs(seconds);
            var offsetStr = $"{absSeconds / 3600}:{(absSeconds % 3600) / 60:D2}";
            var offsetSuffix = seconds > 0 ? "ahead" : "behind";
            return $"{offsetStr} {offsetSuffix}";
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
                dates.Add(FormatTime(ITimeZone.TimeFormat.DATE_LONG,interval));
            }
            return string.Join(",",dates);
        }

        private bool IsBetween(DateTimeZone zone, LocalDateTime time, Instant start, Instant end)
        {
            Instant targetInstant = time.InZoneLeniently(zone).ToInstant();
            return targetInstant >= start && targetInstant < end;
        }

        private string FormatTime(ITimeZone.TimeFormat format, LocalDateTime time)
        {
            string strTimeFormat = "h:mm tt";
            switch (format)
            {
                case ITimeZone.TimeFormat.TIME_SHORT_AMPM:
                    strTimeFormat = "h:mm tt";
                    break;
                case ITimeZone.TimeFormat.TIME_SHORT_24:
                    strTimeFormat = "HH:mm";
                    break;
                case ITimeZone.TimeFormat.DAY_SHORT:
                    strTimeFormat = "ddd";
                    break;
                case ITimeZone.TimeFormat.DATE_LONG:
                    // TODO - take account of the device culture
                    strTimeFormat = "dd MMM yyyy";
                    break;
                case ITimeZone.TimeFormat.DATE_TIME_LONG:
                    strTimeFormat = "yyyy MMM dd HH:mm";
                    break;
                case ITimeZone.TimeFormat.DAY_TIME_SHORT_AMPM:
                    strTimeFormat = "ddd dd, h:mm tt";
                    break;
                case ITimeZone.TimeFormat.DAY_TIME_SHORT_24:
                    strTimeFormat = "ddd dd, HH:mm";
                    break;
            }
            return time.ToString(strTimeFormat, CultureInfo.CurrentCulture);
        }

        public string ToLocalTimeFromUtcFormatted(DateTime utctime, ITimeZone.TimeFormat format)
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
