using NodaTime.TimeZones;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Worldolio.Data.Logging;

namespace WorldolioCLI
{
    internal class ApplicationHelper
    {
        public enum Command
        {
            Unknown,
            Find,
            CityList,
            CountryList
        }

        static public void OutputToConsole(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }

        static public string GetCodeVersion()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            return name.Version?.ToString() ?? "UNKNOWN";
        }

        static public void DisplayBanner()
        {
            OutputToConsole($"WorldolioCLI v{GetCodeVersion()}");
        }

        public static string GetIanaTzDatabaseVersion()
        {
            // Access the version via the default TZDB source
            return TzdbDateTimeZoneSource.Default.TzdbVersion;
        }

        static public void DisplayEnvironment()
        {
            OutputToConsole($"Running on .NET CLR: {Environment.Version.ToString()}");
            OutputToConsole($"{RuntimeInformation.OSDescription}, Framework: {RuntimeInformation.FrameworkDescription}, OS: {RuntimeInformation.OSArchitecture}, Processor: {RuntimeInformation.ProcessArchitecture}");
            OutputToConsole($"TZ Database: {GetIanaTzDatabaseVersion()}");
        }

        static public void DisplayUsage()
        {
            OutputToConsole("Usage: WorldolioCLI <command> <param> <date> <time>");
            OutputToConsole("Where");
            OutputToConsole("  <command> = command to execute. find | citylist | countrylist");
            OutputToConsole("  <param> = depends on <command>");
            OutputToConsole("    find = a string to look for in city or country name");
            OutputToConsole("    citylist = list of numeric city ids to display");
            OutputToConsole("    countrylist = list of country ids to display");
            OutputToConsole("  <date> = date to use in yyyy-mm-dd format, default is today");
            OutputToConsole("  <time> = time to use in hh:mm 24 hour format, default is the current time");
        }
        public static Command GetCommand(string arg)
        {
            switch (arg.ToLower())
            {
                case "find":
                    return Command.Find;
                case "citylist":
                    return Command.CityList;
                case "countrylist":
                    return Command.CountryList;
            }
            return Command.Unknown;
        }

        public static string[] GetStringList(string arg)
        {
            return arg.Split(',');
        }

        public static long[] GetLongList(string arg)
        {
            var strings = GetStringList(arg);
            var longs = new long[strings.Length];
            int index = 0;
            foreach ( var s in strings )
            {
                if (long.TryParse(s, out long result))
                {
                    longs[index] = result;
                }
                else
                {
                    longs[index] = 0;
                }
                index++;
            }
            return longs;
        }

        public static DateTime ProcessDateTimeArgs(ILogger logger, string dateStr, string? timeStr, DateTime defaultDate)
        {
            // Exact format pattern: d = day, M = month, y = year, H = 24-hr hour, m = minute
            string format = "yyyy-MM-dd HH:mm";
            var mins = timeStr == null ? defaultDate.ToString("HH:mm") : timeStr;
            var input = $"{dateStr} {mins}";
            logger.Info(() => $"date arg = {input}");
            return DateTime.ParseExact(input, format, CultureInfo.InvariantCulture);
        }
    }
}
