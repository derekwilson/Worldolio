using Worldolio.Data.Exceptions;
using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    public class City
    {
        [Column(Name = "cty_id")]
        public long Id { get; set; }

        [Column(Name = "cty_displayname")]
        public required string DisplayName { get; set; }

        // this is always initialiased by the repository so we can treat is as if it is not nullable
        public Country Country { get; set; } = default!;

        [Column(Name = "cty_windowstzindex")]
        public int WindowsTzIndex { get; set; }

        [Column(Name = "cty_areacode")]
        public string? AreaCode { get; set; }

        [Column(Name = "cty_latitude")]
        public int Latitude { get; set; }

        [Column(Name = "cty_longitude")]
        public int Longitude { get; set; }

        // this is always initialiased by the repository so we can treat is as if it is not nullable
        public Position Position { get; set; } = default!;

        [Column(Name = "cty_iataairportcode")]
        public required string IataAirportCode { get; set; }

        [Column(Name = "cty_icaoairportcode")]
        public required string IcaoAirportCode { get; set; }

        [Column(Name = "cty_ianatz")]
        public required string IanaTz { get; set; }

        // when we shallow load this object then this is not initialsed
        public ITimeZone? TimeZone { get; set; } = default!;

        public Distance GetDistance(Position pos)
        {
            return GeoCalculator.GetDistance(Position, pos);
        }

        public string GetSunrise(DateTime today, ITimeZone.TimeFormat format)
        {
            if (TimeZone is null)
            {
                throw new ShallowObjectException("TimeZone is not initialised");
            }
            var sunriseUtc = GeoCalculator.GetSunriseInUtc(today, Position);
            return TimeZone.ToLocalTimeFromUtcFormatted(sunriseUtc, format);
        }

        public string GetSunset(DateTime today, ITimeZone.TimeFormat format)
        {
            if (TimeZone is null)
            {
                throw new ShallowObjectException("TimeZone is not initialised");
            }
            var sunsetUtc = GeoCalculator.GetSunsetInUtc(today, Position);
            return TimeZone.ToLocalTimeFromUtcFormatted(sunsetUtc, format);
        }

        public string GetNoon(DateTime today, ITimeZone.TimeFormat format)
        {
            if (TimeZone is null)
            {
                throw new ShallowObjectException("TimeZone is not initialised");
            }
            var noonUtc = GeoCalculator.GetSolarNoonInUtc(today, Position.Longitude);
            return TimeZone.ToLocalTimeFromUtcFormatted(noonUtc, format);
        }

        public string GetMoonrise(DateTime today, ITimeZone.TimeFormat format)
        {
            (DateTime? rise, DateTime? set, bool alwaysUp, bool alwaysDown) = GeoCalculator.GetMoonRiseAndSetInUtc(today, Position);
            return FormatMoonState(rise, alwaysUp, alwaysDown, format);
        }

        public string GetMoonset(DateTime today, ITimeZone.TimeFormat format)
        {
            (DateTime? rise, DateTime? set, bool alwaysUp, bool alwaysDown) = GeoCalculator.GetMoonRiseAndSetInUtc(today, Position);
            return FormatMoonState(set, alwaysUp, alwaysDown, format);
        }

        private string FormatMoonState(DateTime? eventUtc, bool alwaysUp, bool alwaysDown, ITimeZone.TimeFormat format)
        {
            if (TimeZone is null)
            {
                throw new ShallowObjectException("TimeZone is not initialised");
            }

            if (eventUtc != null)
            {
                return TimeZone.ToLocalTimeFromUtcFormatted(eventUtc.Value, format);
            }

            if (alwaysUp)
            {
                return "Always Up";
            }
            if (alwaysDown)
            {
                return "Always Up";
            }
            return "None";
        }

        public string GetCurrentTimeFormatted(DateTime time, City? referenceCity, ITimeZone.TimeFormat format)
        {
            if (referenceCity == null)
            {
                // maybe revert to the system TZ
                return "UNKNOWN";
            }
            else
            {
                if (referenceCity.TimeZone is null || TimeZone is null)
                {
                    throw new ShallowObjectException("TimeZone is not initialised");
                }
                return TimeZone.ToLocalTimeFormatted(time, referenceCity.TimeZone, format);
            }
        }

        public string GetFormattedOffset(DateTime time, City? referenceCity)
        {
            if (referenceCity == null)
            {
                return "";
            }
            else
            {
                if (referenceCity.TimeZone is null || TimeZone is null)
                {
                    throw new ShallowObjectException("TimeZone is not initialised");
                }
                return TimeZone.GetFormattedOffset(time, referenceCity.TimeZone);
            }

        }
    }
}
