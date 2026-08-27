using System.Diagnostics;
using System.Reflection;
using Worldolio.Data.DependencyInjection;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
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
            await DisplayData();
        }

        private static IContainer _container = null!;

        private static ILogger _logger = null!;
        private static IDriveSideRepository _drivesideRepository = null!;
        private static ICountryRepository _countriesRepository = null!;
        private static ICityRepository _citiesRepository = null!;
        private static ISchemaRevisionAuditRepository _sraRepository = null!;

        static void Init()
        {
            var loggerFactory = new NLogLoggerFactory();
            _logger = loggerFactory.Logger;
            _logger.Info(() => $"WorldolioDataChecker, v{GetCodeVersion()}, Running on .NET CLR: {Environment.Version.ToString()}");
            SetupExceptionHandler();

            DapperExtensions.AttachMappers();
            _container = Registration.GetEmptyContainer();
            Registration.RegisterFileDbConnection(_container, "./worldolio.sqlite");
            Registration.RegisterServices(_container, _logger);

            var diLogger = _container.Resolve<ILogger>();
            diLogger.Info(() => $"DI init OK");

            _citiesRepository = _container.Resolve<ICityRepository>();
            _countriesRepository = _container.Resolve<ICountryRepository>();
            _drivesideRepository = _container.Resolve<IDriveSideRepository>();
            _sraRepository = _container.Resolve<ISchemaRevisionAuditRepository>();
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

        static async Task DisplayData()
        {
            await DisplayDriveSide(-1);

            //await DisplayCountries(null);
            await DisplayCountriesWithCities(null, false);

            await DisplayCities(null);
            await DisplayCountriesWithCities("NZ", true);

            await DisplaySchemaRevisionAudit();
        }

        private static async Task DisplaySchemaRevisionAudit()
        {
            var versions = _sraRepository.GetDatabaseSchemaVersions();
            Console.WriteLine($"SQLite version: {versions.Item1}, Schema Version: {versions.Item2}");

            ICollection<SchemaRevisionAudit> allSra = await _sraRepository.GetAllAsync();
            foreach (SchemaRevisionAudit item in allSra)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{item.Timestamp}, {item.Description}");
                Console.ResetColor();
            }
            Console.WriteLine($"SRA count = {allSra.Count}");
        }

        private static async Task DisplayDriveSide(int id)
        {
            if (id < 0)
            {
                ICollection<DriveSide> allDriveSide = await _drivesideRepository.GetAllAsync();
                foreach (DriveSide ds in allDriveSide)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"DriveSide {ds.Id} {ds.Description}");
                    Console.ResetColor();
                }
                Console.WriteLine("DriveSide count = {0}", allDriveSide.Count);
            }
            else
            {
                DriveSide? ds = await _drivesideRepository.GetByIdAsync(id);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"DriveSide {ds?.Id} {ds?.Description}");
                Console.ResetColor();
            }
        }

        private static async Task DisplayCountries(string? iso2name)
        {
            if (string.IsNullOrEmpty(iso2name))
            {
                ICollection<Country> countries = await _countriesRepository.GetAllAsync();
                foreach (Country c in countries)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Country {c.Iso2Name}, {c.Iso3Name}, {c.DisplayName}, {c.DriveSide.Description}, Cities = {c.Cities.Count}");
                    Console.ResetColor();
                }
                Console.WriteLine("Countires count = {0}", countries.Count);
            }
            else
            {
                Country? c = await _countriesRepository.GetByIdAsync(iso2name);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Country {c?.Iso2Name}, {c?.Iso3Name}, {c?.DisplayName}, {c?.DriveSide.Description}, Cities = {c?.Cities.Count}");
                Console.ResetColor();
            }

        }

        private static async Task DisplayCountriesWithCities(string? iso2name, bool showCities)
        {
            if (string.IsNullOrEmpty(iso2name))
            {
                ICollection<Country> countries = await _countriesRepository.GetAllWithCitiesAsync();
                foreach (Country c in countries)
                {
                    DisplayOneCountryWithCities(c, showCities);
                }
                Console.WriteLine("Countires count = {0}", countries.Count);
            }
            else
            {
                Country? c = await _countriesRepository.GetByIdWithCitiesAsync(iso2name);
                if (c != null)
                {
                    DisplayOneCountryWithCities(c, showCities);
                }
            }
        }

        private static void DisplayOneCountryWithCities(Country c, bool showCities)
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

        private static async Task DisplayCities(long[]? ids)
        {
            ICollection<City> cities = ids == null ? await _citiesRepository.GetAllAsync() : await _citiesRepository.GetByIdsAsync(ids);

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
    }
}
