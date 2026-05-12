using System.Diagnostics;
using System.Reflection;
using TimeZoneConverter;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;

namespace WorldolioPOC
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
            OutputToConsole($"WorldolioPOC v{GetCodeVersion()}");
        }

        static private void DisplayEnvironment()
        {
            OutputToConsole($"Running on .NET CLR: {Environment.Version.ToString()}");
        }

        private static ILogger? Logger;
        private static IConnectionFactory _connectionFactory;
        private static ICityRepository _citiesRepository;
        private static ISystemTimeProvider _systemTimeProvider;

        static void Init()
        {
            DapperExtensions.AttachMappers();
            _systemTimeProvider = new SystemTimeProvider();
            var timeZoneFactory = new TimeZoneFactory(_systemTimeProvider);
            _connectionFactory = new LocalFileDbConnectionFactory("./worldolio.sqlite");
            _citiesRepository = new CityRepository(_connectionFactory, timeZoneFactory);
        }

        private static async Task Main(string[] args)
        {
            DisplayBanner();
            DisplayEnvironment();
            Init();

            //DisplayIanaId("GMT Standard Time");
            //TZNameConverter.DisplayAllTzs();

            Moon.DisplayMoonPhase();
            Moon.DisplayMoonRiseSet(_citiesRepository, 458, _systemTimeProvider.GetUtcNow());
        }

        private static TimeZoneInfo GetTZInfo(string tzName)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(tzName);
        }

        private static void DisplayTz(string tzName) 
        { 
            var info = GetTZInfo(tzName);
            if (info != null)
            {
                OutputToConsole($"{info.DisplayName}, IANA {info.HasIanaId}, {info.Id}");
            }
            else 
            {
                OutputToConsole("cannot find TZ");
            }
        }

        private static void DisplayIanaId(string tzName)
        {
            var name = TZConvert.WindowsToIana(tzName);
            OutputToConsole($"{tzName} = IANA {name}");
        }

    }
}


