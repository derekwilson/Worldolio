#if ANDROID
using Android.Content.PM;
using AndroidX.Core.Content.PM;
#endif
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
            return $"{AppInfo.Current.Version.Major}.{AppInfo.Current.Version.Minor}.{AppInfo.Current.Version.Build} ({GetVersionCode()}) ({GetBuildType()}) - {GetGitHash()}";
        }

        private string GetVersionCode()
        {
#if ANDROID
            var context = Android.App.Application.Context;
            PackageInfo? package = (
                //Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu ?
                OperatingSystem.IsAndroidVersionAtLeast(33) ?
                context.PackageManager?.GetPackageInfo(context.PackageName ?? "", PackageManager.PackageInfoFlags.Of((long)0)) :
                context.PackageManager?.GetPackageInfo(context.PackageName ?? "", 0)
            );
            long longVersionCode = package != null ? PackageInfoCompat.GetLongVersionCode(package) : 0;
            return longVersionCode.ToString();
#else
            return AppInfo.Current.Version.Revision.ToString();
#endif
        }

        private string GetBuildType()
        {
#if DEBUG
            return "Debug";
#else
            return "Release";
#endif
        }

        private string GetGitHash()
        {
            //return "UNKNOWN";
            return GitInfo.Hash;
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
