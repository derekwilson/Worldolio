using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using static Worldolio.Data.Model.TimeZone;

namespace WorldolioCLI
{
    internal class CityHelper
    {
        public static async Task DisplayCityGrid(ICityRepository citiesRepository, long[] ids, bool showNearby, DateTime now)
        {
            if (ids.Length < 1)
            {
                Console.WriteLine($"no cities in the list");
                return;
            }
            var homeId = ids[0];
            Console.WriteLine($"City Grid = {homeId}, [{string.Join(',', ids)}]");
            Console.WriteLine($"Date Time = {now.ToString("dd MMM yyyy, HH:mm")}");

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
                Console.WriteLine($"   {city.TimeZone.GetFormattedLocalTime(now, TimeFormat.DAY_SHORT)} {city.TimeZone.GetFormattedLocalTime(now, TimeFormat.TIME_SHORT_AMPM)}");
                Console.WriteLine($"   {city.TimeZone.GetFormattedOffset(now, home.TimeZone)}, DST {city.TimeZone.GetDSTDatesForDisplay(now)}, TZ {city.IanaTz}");
                var nearby = await citiesRepository.GetNearbyCitiesAsync(city, new Distance(500, Distance.Units.Miles));
                if (showNearby)
                {
                    foreach (City city2 in nearby)
                    {
                        Console.WriteLine($"     City {city2.Id}, {city2.DisplayName}, {city2.Country.DisplayName}, {city.GetDistance(city2.Position).ToString(Distance.Units.Kilometers)}");
                    }
                }
                Console.WriteLine($"   Nearby cities count = {nearby.Count}");
                Console.WriteLine($"   Sunrise: {city.GetSunrise(now, TimeFormat.TIME_SHORT_AMPM)}, Sunset: {city.GetSunset(now, TimeFormat.TIME_SHORT_AMPM)}, Noon: {city.GetNoon(now, TimeFormat.TIME_SHORT_AMPM)}");
                Console.WriteLine($"   Moonrise: {city.GetMoonrise(now, TimeFormat.DAY_TIME_SHORT_AMPM)}, Moonset: {city.GetMoonset(now, TimeFormat.DAY_TIME_SHORT_AMPM)}");
                Console.ResetColor();
            }
            var invalidCount = cities.Count(c => !c.TimeZone.IsValid);
            Console.WriteLine($"Cities count = {cities.Count}, invalid TZ = {invalidCount}");
        }

        internal static async Task FindCities(ICountryRepository countriesRepository, ICityRepository citiesRepository, string searchName)
        {
            ICollection<City> cities = await citiesRepository.FindByNameAsync(searchName);

            if (cities != null && cities.Count > 0)
            {
                Console.WriteLine($"{cities.Count} cities match with '{searchName}' ");
                foreach (City city in cities)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{city.DisplayName}, {city.Country.DisplayName}, ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"ID {city.Id}");
                    Console.ResetColor();
                }
            }

            ICollection<Country> countries = await countriesRepository.FindByNameAsync(searchName);
            if (countries != null && countries.Count > 0)
            {
                Console.WriteLine($"{countries.Count} countries match with '{searchName}' ");
                foreach (Country country in countries)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{country.Iso2Name}, {country.Iso3Name}, {country.DisplayName}");
                    foreach (City countryCity in country.Cities)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"  {countryCity.DisplayName}, ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"  ID {countryCity.Id}");
                    }
                    Console.ResetColor();
                }
            }

            Console.WriteLine($"Hit count = {cities?.Count + countries?.Count}");
        }
    }
}
