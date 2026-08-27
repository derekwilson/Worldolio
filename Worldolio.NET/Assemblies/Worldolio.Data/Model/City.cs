using Worldolio.Data.Utility;

namespace Worldolio.Data.Model
{
    public class City
    {
        [Column(Name = "cty_id")]
        public long Id { get; set; }

        [Column(Name = "cty_displayname")]
        public required string DisplayName { get; set; }

        public Country Country { get; set; } = default!;

        [Column(Name = "cty_windowstzindex")]
        public int WindowsTzIndex { get; set; }

        [Column(Name = "cty_areacode")]
        public string? AreaCode { get; set; }

        [Column(Name = "cty_latitude")]
        public int Latitude { get; set; }

        [Column(Name = "cty_longitude")]
        public int Longitude { get; set; }

        public Position Position { get; set; } = default!;

        [Column(Name = "cty_iataairportcode")]
        public required string IataAirportCode { get; set; }

        [Column(Name = "cty_icaoairportcode")]
        public required string IcaoAirportCode { get; set; }

        [Column(Name = "cty_ianatz")]
        public required string IanaTz { get; set; }

        public TimeZone TimeZone { get; set; } = default!;

        public Distance GetDistance(Position pos)
        {
            return GeoCalculator.GetDistance(Position, pos);
        }

        public string GetSunrise(TimeZone.TimeFormat format)
        {
            return GetSunrise(TimeZone.GetUtcNow(), format);
        }

        public string GetSunrise(DateTime today, TimeZone.TimeFormat format)
        {
            var sunriseUtc = GeoCalculator.GetSunriseInUtc(today, Position);
            return TimeZone.ToLocalTimeFormatted(sunriseUtc, format);
        }

        public string GetSunset(TimeZone.TimeFormat format)
        {
            return GetSunset(TimeZone.GetUtcNow(), format);
        }

        public string GetSunset(DateTime today, TimeZone.TimeFormat format)
        {
            var sunsetUtc = GeoCalculator.GetSunsetInUtc(today, Position);
            return TimeZone.ToLocalTimeFormatted(sunsetUtc, format);
        }

        public string GetNoon(TimeZone.TimeFormat format)
        {
            return GetNoon(TimeZone.GetUtcNow(), format);
        }

        public string GetNoon(DateTime today, TimeZone.TimeFormat format)
        {
            var noonUtc = GeoCalculator.GetSolarNoonInUtc(today, Position.Longitude);
            return TimeZone.ToLocalTimeFormatted(noonUtc, format);
        }

        public string GetMoonrise(TimeZone.TimeFormat format)
        {
            return GetMoonrise(TimeZone.GetUtcNow(), format);
        }

        public string GetMoonrise(DateTime today, TimeZone.TimeFormat format)
        {
            (DateTime? rise, DateTime? set, bool alwaysUp, bool alwaysDown) = GeoCalculator.GetMoonRiseAndSetInUtc(today, Position);
            return FormatMoonState(rise, alwaysUp, alwaysDown, format);
        }

        public string GetMoonset(TimeZone.TimeFormat format)
        {
            return GetMoonset(TimeZone.GetUtcNow(), format);
        }

        public string GetMoonset(DateTime today, TimeZone.TimeFormat format)
        {
            (DateTime? rise, DateTime? set, bool alwaysUp, bool alwaysDown) = GeoCalculator.GetMoonRiseAndSetInUtc(today, Position);
            return FormatMoonState(set, alwaysUp, alwaysDown, format);
        }

        private string FormatMoonState(DateTime? eventUtc, bool alwaysUp, bool alwaysDown, TimeZone.TimeFormat format)
        {
            if (eventUtc != null)
            {
                return TimeZone.ToLocalTimeFormatted(eventUtc.Value, format);
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

    }
}
