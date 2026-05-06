using NodaTime;
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

        public string GetSunrise()
        {
            return GetSunrise(TimeZone.GetUtcNow());
        }

        public string GetSunrise(ZonedDateTime today)
        {
            var sunriseUtc = GeoCalculator.GetSunriseInUtc(today, Position);
            return TimeZone.ToLocalTimeFormatted(sunriseUtc);
        }

        public string GetSunset()
        {
            return GetSunset(TimeZone.GetUtcNow());
        }

        public string GetSunset(ZonedDateTime today)
        {
            var sunsetUtc = GeoCalculator.GetSunsetInUtc(today, Position);
            return TimeZone.ToLocalTimeFormatted(sunsetUtc);
        }

        public string GetNoon()
        {
            return GetNoon(TimeZone.GetUtcNow());
        }

        public string GetNoon(ZonedDateTime today)
        {
            var noonUtc = GeoCalculator.GetSolarNoonInUtc(today, Position.Longitude);
            return TimeZone.ToLocalTimeFormatted(noonUtc);
        }
    }
}
