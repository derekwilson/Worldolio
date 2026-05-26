using NodaTime.TimeZones;
using WorldolioMauiPOC.Logging;

namespace WorldolioMauiPOC.Utility
{
    public interface IEnvironmentInformationProvider
    {
        string GetAppVersion();
        string GetDatabasePath();
        string GetPackageName();
        string GetLogfileLocation();
        string GetIanaTzDatabaseVersion();
    }

    public class EnvironmentInformationProvider : IEnvironmentInformationProvider
    {
        public string GetAppVersion()
        {
            return $"{AppInfo.Current.Version.Major}.{AppInfo.Current.Version.Minor}.{AppInfo.Current.Version.Build} ({AppInfo.Current.Version.Revision})";
        }

        public string GetDatabasePath()
        {
            return Data.DatabaseHelper.GetDatabaseFilePath();
        }

        public string GetIanaTzDatabaseVersion()
        {
            // Access the version via the default TZDB source
            return TzdbDateTimeZoneSource.Default.TzdbVersion;
        }

        public string GetLogfileLocation()
        {
            // if we are not using NLog then this need to be changed
            return NLogMauiLoggerFactory.GetLoggingDir();
        }

        public string GetPackageName()
        {
            return $"{AppInfo.Current.PackageName}";
        }
    }
}
