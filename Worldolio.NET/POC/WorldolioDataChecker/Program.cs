using System.Diagnostics;
using System.Reflection;
using Worldolio.Data;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;

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

        private static IConnectionFactory _connectionFactory;
        private static IDriveSideRepository _drivesideRepository;
        private static ICountryRepository _countriesRepository;
        private static ICityRepository _citiesRepository;

        static void Init()
        {
            DapperExtensions.AttachMappers();
            var instantProvider = new InstantProvider();
            var timeZoneFactory = new TimeZoneFactory(instantProvider);
            _connectionFactory = new LocalFileDbConnectionFactory("./worldolio.sqlite");
            _drivesideRepository = new DriveSideRepository(_connectionFactory);
            _countriesRepository = new CountryRepository(_connectionFactory);
            _citiesRepository = new CityRepository(_connectionFactory, timeZoneFactory);
        }

        static void DisplayData()
        {
            DisplayDriveSide(-1);
            DisplayCountries(null);
            DisplayCountriesWithCities(null);
            DisplayCities(null);

            DisplayCountriesWithCities("NZ");
            long[] cityIds = [458, 252, 324, 313, 477, 79, 320];
            DisplayCities(cityIds);
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

        private static void DisplayCountriesWithCities(string? iso2name)
        {
            ICollection<Country> countries = String.IsNullOrEmpty(iso2name) ? _countriesRepository.GetAllWithCities() : _countriesRepository.GetAllWithCitiesById(iso2name);

            foreach (Country c in countries)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Country {c.Iso2Name}, {c.Iso3Name}, {c.DisplayName}, {c.DriveSide.Description}, Cities = {c.Cities.Count}");
                Console.ResetColor();
            }
            Console.WriteLine("Countires count = {0}", countries.Count);
        }

        private static void DisplayCities(long[]? ids)
        {
            ICollection<City> cities = ids == null ? _citiesRepository.GetAll() : _citiesRepository.GetByIds(ids);

            foreach (City city in cities)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"City {city.Id}, {city.DisplayName}, {city.Country.DisplayName}, Pos {city.Position.ToString(true)} Drives {city.Country.DriveSide.Description}, TZ {city.IanaTz}, {city.TimeZone.GetNow()}");
                Console.ResetColor();
            }
            var invalidCount = cities.Count(c => !c.TimeZone.IsValid);
            Console.WriteLine($"Cities count = {cities.Count}, invalid TZ = {invalidCount}");
        }
    }
}
