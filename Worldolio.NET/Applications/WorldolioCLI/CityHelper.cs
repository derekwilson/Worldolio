using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using static Worldolio.Data.Model.TimeZone;

namespace WorldolioCLI
{
    internal class CityHelper
    {
        public static async Task DisplayCityGrid(ICityRepository citiesRepository, long[] ids, bool showNearby)
        {
            if (ids.Length < 1)
            {
                Console.WriteLine($"no cities in the list");
                return;
            }
            var homeId = ids[0];
            Console.WriteLine($"City Grid = {homeId}, [{string.Join(',', ids)}]");

            var home = await citiesRepository.GetByIdAsync(homeId);
            if (home == null)
            {
                throw new Exception($"Bad home city. ID: {homeId}");
            }
            ICollection<City> cities = await citiesRepository.GetByIdsAsync(ids);

            foreach (City city in cities)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"City {city.Id}, {city.DisplayName}, {city.Country.DisplayName}, Pos {city.Position.ToString(true)} Drives {city.Country.DriveSide.Description}");
                Console.WriteLine($"   {city.TimeZone.GetFormattedLocalTime(TimeFormat.DAY_SHORT)} {city.TimeZone.GetFormattedLocalTime(TimeFormat.TIME_SHORT_AMPM)}");
                Console.WriteLine($"   {city.TimeZone.GetFormattedOffset(home.TimeZone)}, DST {city.TimeZone.GetDSTDatesForDisplay()}, TZ {city.IanaTz}");
                var nearby = await citiesRepository.GetNearbyCitiesAsync(city, new Distance(500, Distance.Units.Miles));
                if (showNearby)
                {
                    foreach (City city2 in nearby)
                    {
                        Console.WriteLine($"     City {city2.Id}, {city2.DisplayName}, {city2.Country.DisplayName}, {city.GetDistance(city2.Position).ToString(Distance.Units.Kilometers)}");
                    }
                }
                Console.WriteLine($"   Nearby cities count = {nearby.Count}");
                Console.WriteLine($"   Sunrise: {city.GetSunrise(TimeFormat.TIME_SHORT_AMPM)}, Sunset: {city.GetSunset(TimeFormat.TIME_SHORT_AMPM)}, Noon: {city.GetNoon(TimeFormat.TIME_SHORT_AMPM)}");
                Console.WriteLine($"   Moonrise: {city.GetMoonrise(TimeFormat.DAY_TIME_SHORT_AMPM)}, Moonset: {city.GetMoonset(TimeFormat.DAY_TIME_SHORT_AMPM)}");
                Console.ResetColor();
            }
            var invalidCount = cities.Count(c => !c.TimeZone.IsValid);
            Console.WriteLine($"Cities count = {cities.Count}, invalid TZ = {invalidCount}");
        }
    }
}
