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
                var localTime = new DateTime(year, month, day, 00, 00, 00);
                DateTime dtUtc = DateTime.SpecifyKind(localTime, DateTimeKind.Utc);

                Console.WriteLine($"Moon %, {day} {month} {year} = {GeoCalculator.GetFormattedIlluminatedFractionOfMoon(dtUtc)}");
            }
        }

        public static async Task DisplayMoonRiseSet(ICityRepository citiesRepository, int homeId, DateTime date)
        {
            var home = await citiesRepository.GetByIdAsync(homeId);
            if (home == null)
            {
                throw new Exception($"Bad home city. ID: {homeId}");
            }
            var moon = GeoCalculator.GetMoonRiseAndSetInUtc(date, home.Position);
            var rise = moon.Item1 == null ? "None" : moon.Item1.ToString();
            var set = moon.Item2 == null ? "None" : moon.Item2.ToString();
            Console.WriteLine($"Moon Rise {rise} Set {set}");

            var localTime = new DateTime(2026, 5, 10, 00, 00, 0);
            moon = GeoCalculator.GetMoonRiseAndSetInUtc(localTime, home.Position);
            rise = moon.Item1 == null ? "None" : moon.Item1.ToString();
            set = moon.Item2 == null ? "None" : moon.Item2.ToString();
            Console.WriteLine($"Moon Rise2 {rise} Set {set}");

            Console.WriteLine($"   Moonrise: {home.GetMoonrise(date, TimeFormat.DATE_TIME_LONG)}, Moonset: {home.GetMoonset(date, TimeFormat.DATE_TIME_LONG)}");
        }
    }
}
