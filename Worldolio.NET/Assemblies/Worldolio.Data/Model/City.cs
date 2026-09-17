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

        public ITimeZone TimeZone { get; set; } = default!;

        public Distance GetDistance(Position pos)
        {
            return GeoCalculator.GetDistance(Position, pos);
        }

        public string GetSunrise(DateTime today, ITimeZone.TimeFormat format)
        {
            var sunriseUtc = GeoCalculator.GetSunriseInUtc(today, Position);
            return TimeZone.ToLocalTimeFromUtcFormatted(sunriseUtc, format);
        }

        public string GetSunset(DateTime today, ITimeZone.TimeFormat format)
        {
            var sunsetUtc = GeoCalculator.GetSunsetInUtc(today, Position);
            return TimeZone.ToLocalTimeFromUtcFormatted(sunsetUtc, format);
        }

        public string GetNoon(DateTime today, ITimeZone.TimeFormat format)
        {
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

    }
}
