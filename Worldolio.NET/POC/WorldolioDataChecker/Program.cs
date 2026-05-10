using System.Diagnostics;
using System.Reflection;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;
using static Worldolio.Data.Model.TimeZone;

namespace WorldolioDataChecker
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
            DisplayData();
        }

        private static ILogger? Logger;
        private static IConnectionFactory _connectionFactory;
        private static IDriveSideRepository _drivesideRepository;
        private static ICountryRepository _countriesRepository;
        private static ICityRepository _citiesRepository;

        static void Init()
        {
            var loggerFactory = new NLoggerLoggerFactory();
            Logger = loggerFactory.Logger;
            Logger.Info(() => $"WorldolioDataChecker, v{GetCodeVersion()}, Running on .NET CLR: {Environment.Version.ToString()}");

            SetupExceptionHandler();

            DapperExtensions.AttachMappers();
            var systemTimeProvider = new SystemTimeProvider();
            var timeZoneFactory = new TimeZoneFactory(systemTimeProvider);
            _connectionFactory = new LocalFileDbConnectionFactory("./worldolio.sqlite");
            _drivesideRepository = new DriveSideRepository(_connectionFactory);
            _countriesRepository = new CountryRepository(_connectionFactory);
            _citiesRepository = new CityRepository(_connectionFactory, timeZoneFactory);
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
            Logger?.LogException(() => "TaskSchedulerOnUnobservedTaskException", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("EXCEPTION NOT PROVIDED");
            Logger?.LogException(() => "CurrentDomain_UnhandledException", ex);
        }

        #endregion

        static void DisplayData()
        {
            DisplayDriveSide(-1);

            //DisplayCountries(null);
            //DisplayCountriesWithCities(null, false);

            DisplayCities(null);
            DisplayCountriesWithCities("NZ", true);

            long[] cityIds = [458, 252, 324, 313, 477, 79, 320, 279, 180];
            DisplayCityGrid(458,cityIds,false);
        }

        private static void DisplayDriveSide(int id)
        {
            ICollection<DriveSide> driveSide = id < 0 ? _drivesideRepository.GetAll() : _drivesideRepository.GetById(id);

            foreach (DriveSide ds in driveSide)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"DriveSide {ds.Id} {ds.Description}");
                Console.ResetColor();
            }
            Console.WriteLine("DriveSide count = {0}", driveSide.Count);
        }

        private static void DisplayCountries(string? iso2name)
        {
            ICollection<Country> countries = String.IsNullOrEmpty(iso2name) ? _countriesRepository.GetAll() : _countriesRepository.GetById(iso2name);

            foreach (Country c in countries)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Country {c.Iso2Name}, {c.Iso3Name}, {c.DisplayName}, {c.DriveSide.Description}, Cities = {c.Cities.Count}");
                Console.ResetColor();
            }
            Console.WriteLine("Countires count = {0}", countries.Count);
        }

        private static void DisplayCountriesWithCities(string? iso2name, bool showCities)
        {
            ICollection<Country> countries = String.IsNullOrEmpty(iso2name) ? _countriesRepository.GetAllWithCities() : _countriesRepository.GetAllWithCitiesById(iso2name);

            foreach (Country c in countries)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Country {c.Iso2Name}, {c.Iso3Name}, {c.DisplayName}, {c.DriveSide.Description}, Cities = {c.Cities.Count}");
                if (showCities)
                {
                    foreach (City city in c.Cities)
                    {
                        Console.WriteLine($"     City {city.Id}, {city.DisplayName}, {city.Country.DisplayName}");
                    }
                }
                Console.ResetColor();
            }
            Console.WriteLine("Countires count = {0}", countries.Count);
        }

        private static void DisplayCities(long[]? ids)
        {
            ICollection<City> cities = ids == null ? _citiesRepository.GetAll() : _citiesRepository.GetByIds(ids);

            foreach (City city in cities)
            {
                if (city.TimeZone.IsValid)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.WriteLine($"City {city.Id}, {city.DisplayName}, {city.Country.DisplayName}, Pos {city.Position.ToString(true)} Drives {city.Country.DriveSide.Description}");
                Console.WriteLine($"   TZ {city.IanaTz}, {city.TimeZone.GetFormattedLocalTime(TimeFormat.TIME_SHORT_AMPM)}, {city.TimeZone.GetDSTDatesForDisplay()}");
                Console.ResetColor();
            }
            var invalidCount = cities.Count(c => !c.TimeZone.IsValid);
            Console.WriteLine($"Cities count = {cities.Count}, invalid TZ = {invalidCount}");
        }

        private static void DisplayCityGrid(long homeId, long[] ids, bool showNearby)
        {
            Console.WriteLine($"City Grid = {homeId}, [{string.Join(',',ids)}]");

            var home = _citiesRepository.GetById(homeId);
            if (home == null)
            {
                throw new Exception($"Bad home city. ID: {homeId}");
            }
            ICollection<City> cities = _citiesRepository.GetByIds(ids);

            foreach (City city in cities)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"City {city.Id}, {city.DisplayName}, {city.Country.DisplayName}, Pos {city.Position.ToString(true)} Drives {city.Country.DriveSide.Description}");
                Console.WriteLine($"   {city.TimeZone.GetFormattedLocalTime(TimeFormat.DAY_SHORT)} {city.TimeZone.GetFormattedLocalTime(TimeFormat.TIME_SHORT_AMPM)}");
                Console.WriteLine($"   {city.TimeZone.GetOffset(home.TimeZone)}, DST {city.TimeZone.GetDSTDatesForDisplay()}, TZ {city.IanaTz}");
                var nearby = _citiesRepository.GetNearbyCities(city, new Distance(500, Distance.Units.Miles));
                if (showNearby)
                {
                    foreach (City city2 in nearby)
                    {
                        Console.WriteLine($"     City {city2.Id}, {city2.DisplayName}, {city2.Country.DisplayName}, {city.GetDistance(city2.Position).ToString(Distance.Units.Kilometers)}");
                    }
                }
                Console.WriteLine($"   Nearby cities count = {nearby.Count}");
                Console.WriteLine($"   Sunrise: {city.GetSunrise(TimeFormat.TIME_SHORT_AMPM)}, Sunset: {city.GetSunset(TimeFormat.TIME_SHORT_AMPM)}, Noon: {city.GetNoon(TimeFormat.TIME_SHORT_AMPM)}");
                Console.ResetColor();
            }
            var invalidCount = cities.Count(c => !c.TimeZone.IsValid);
            Console.WriteLine($"Cities count = {cities.Count}, invalid TZ = {invalidCount}");
        }

    }
}
