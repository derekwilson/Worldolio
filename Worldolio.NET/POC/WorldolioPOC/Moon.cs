using NodaTime;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using static Worldolio.Data.Model.TimeZone;

namespace WorldolioPOC
{
    internal class Moon
    {
        public static void DisplayMoonPhase()
        {
            for (int day = 1; day <= 31; day++)
            {
                Console.WriteLine($"Moon Phase {day} May 2026 = {GeoCalculator.GetMoonPhase(2026, 5, day)}, {GeoCalculator.GetFormattedMoonPhase(2026, 5, day)}");
            }

            for (int day = 1; day <= 30; day++)
            {
                Console.WriteLine($"Moon Phase {day} June 2026 = {GeoCalculator.GetMoonPhase(2026, 6, day)}, {GeoCalculator.GetFormattedMoonPhase(2026, 6, day)}");
            }

            for (int day = 1; day <= 31; day++)
            {
                Console.WriteLine($"Moon Phase {day} July 2026 = {GeoCalculator.GetMoonPhase(2026, 7, day)}, {GeoCalculator.GetFormattedMoonPhase(2026, 7, day)}");
            }

            for (int day = 1; day <= 31; day++)
            {
                Console.WriteLine($"Moon Phase {day} Aug 2026 = {GeoCalculator.GetMoonPhase(2026, 8, day)}, {GeoCalculator.GetFormattedMoonPhase(2026, 8, day)}");
            }

            DisplayOneMonth(31, 5, 2026);
            DisplayOneMonth(30, 6, 2026);
            DisplayOneMonth(31, 7, 2026);
            DisplayOneMonth(31, 8, 2026);
        }

        private static void DisplayOneMonth(int nDays, int month, int year)
        {
            for (int day = 1; day <= nDays; day++)
            {
                var localTime = new LocalDateTime(year, month, day, 00, 00);
                //var zone = DateTimeZoneProviders.Tzdb["Pacific/Auckland"];
                var zone = DateTimeZoneProviders.Tzdb["UTC"];

                // Option A: Be strict (safe if you know it's a valid time)
                ZonedDateTime ztime = localTime.InZoneStrictly(zone);

                Console.WriteLine($"Moon %, {day} {month} {year} = {GeoCalculator.GetFormattedIlluminatedFractionOfMoon(ztime.ToInstant())}");
            }
        }

        public static void DisplayMoonRiseSet(ICityRepository citiesRepository, int homeId, ZonedDateTime date)
        {
            var home = citiesRepository.GetById(homeId);
            if (home == null)
            {
                throw new Exception($"Bad home city. ID: {homeId}");
            }
            var moon = GeoCalculator.GetMoonRiseAndSetInUtc(date, home.Position);
            var rise = moon.Item1 == null ? "None" : moon.Item1.ToString();
            var set = moon.Item2 == null ? "None" : moon.Item2.ToString();
            Console.WriteLine($"Moon Rise {rise} Set {set}");

            var localTime = new LocalDateTime(2026, 5, 10, 00, 00);
            var zone = DateTimeZoneProviders.Tzdb["Pacific/Auckland"];
            //var zone = DateTimeZoneProviders.Tzdb["UTC"];

            // Option A: Be strict (safe if you know it's a valid time)
            ZonedDateTime strictZdt = localTime.InZoneStrictly(zone);

            moon = GeoCalculator.GetMoonRiseAndSetInUtc(strictZdt, home.Position);
            rise = moon.Item1 == null ? "None" : moon.Item1.ToString();
            set = moon.Item2 == null ? "None" : moon.Item2.ToString();
            Console.WriteLine($"Moon Rise2 {rise} Set {set}");

            Console.WriteLine($"   Moonrise: {home.GetMoonrise(strictZdt, TimeFormat.DATE_TIME_LONG)}, Moonset: {home.GetMoonset(strictZdt, TimeFormat.DATE_TIME_LONG)}");
        }
    }
}
