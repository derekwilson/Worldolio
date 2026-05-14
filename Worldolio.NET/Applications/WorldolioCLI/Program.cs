using System.Diagnostics;
using System.Reflection;
using Worldolio.Data.DependencyInjection;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;
using static Worldolio.Data.Model.TimeZone;

namespace WorldolioCLI
{
    internal class Program
    {
        static void OutputToConsole(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }

        static void OutputToLogger(string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        static private string GetCodeVersion()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            return name.Version?.ToString() ?? "UNKNOWN";
        }

        static private void DisplayBanner()
        {
            OutputToConsole($"WorldolioDataChecker v{GetCodeVersion()}");
        }

        static private void DisplayEnvironment()
        {
            OutputToConsole($"Running on .NET CLR: {Environment.Version.ToString()}");
        }

        private static async Task Main(string[] args)
        {
            DisplayBanner();
            DisplayEnvironment();

            Init();

            long[] cityIds = [458, 252, 324, 313, 477, 79, 320, 279, 180, 351];
            await DisplayCityGrid(458, cityIds, false);
        }

        private static IContainer _container = null!;

        private static ILogger _logger = null!;
        private static ICityRepository _citiesRepository = null!;

        static void Init()
        {
            var loggerFactory = new NLogLoggerFactory();
            _logger = loggerFactory.Logger;
            _logger.Info(() => $"WorldolioCli, v{GetCodeVersion()}, Running on .NET CLR: {Environment.Version.ToString()}");
            SetupExceptionHandler();

            DapperExtensions.AttachMappers();
            _container = Registration.GetEmptyContainer();
            Registration.RegisterFileDbConnection(_container, "./worldolio.sqlite");
            Registration.RegisterServices(_container, _logger);

            var diLogger = _container.Resolve<ILogger>();
            diLogger.Info(() => $"DI init OK");

            _citiesRepository = _container.Resolve<ICityRepository>();
        }

        #region Exception Handling

        private static void SetupExceptionHandler()
        {
            // Add handler for non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // Add handler for background threads/tasks
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private static void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger?.LogException(() => "TaskSchedulerOnUnobservedTaskException", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("EXCEPTION NOT PROVIDED");
            _logger?.LogException(() => "CurrentDomain_UnhandledException", ex);
        }

        #endregion

        private static async Task DisplayCityGrid(long homeId, long[] ids, bool showNearby)
        {
            Console.WriteLine($"City Grid = {homeId}, [{string.Join(',', ids)}]");

            var home = await _citiesRepository.GetByIdAsync(homeId);
            if (home == null)
            {
                throw new Exception($"Bad home city. ID: {homeId}");
            }
            ICollection<City> cities = await _citiesRepository.GetByIdsAsync(ids);

            foreach (City city in cities)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"City {city.Id}, {city.DisplayName}, {city.Country.DisplayName}, Pos {city.Position.ToString(true)} Drives {city.Country.DriveSide.Description}");
                Console.WriteLine($"   {city.TimeZone.GetFormattedLocalTime(TimeFormat.DAY_SHORT)} {city.TimeZone.GetFormattedLocalTime(TimeFormat.TIME_SHORT_AMPM)}");
                Console.WriteLine($"   {city.TimeZone.GetFormattedOffset(home.TimeZone)}, DST {city.TimeZone.GetDSTDatesForDisplay()}, TZ {city.IanaTz}");
                var nearby = await _citiesRepository.GetNearbyCitiesAsync(city, new Distance(500, Distance.Units.Miles));
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
