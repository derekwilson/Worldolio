using NLog;
using NLog.Targets;
using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Logging
{
    public class NLogMauiLoggerFactory : ILoggerFactory
    {
        public NLogMauiLoggerFactory()
        {
            LoadConfig();
            // set the log file destination
            SetupLoggingDir(GetLoggingDir());

            // set the loglevel
#if DEBUG
            NLogHelper.SetLoggingLevel(NLog.LogLevel.Trace);
#else
            NLogHelper.SetLoggingLevel(NLog.LogLevel.Error);
#endif
        }

        private void LoadConfig()
        {
            LogManager
                .Setup()
                .RegisterMauiLog()
                .LoadConfigurationFromAssemblyResource(typeof(App).Assembly);
        }

        public static string GetLoggingDir()
        {
#if WINDOWS
            return GetupWindowsLoggingDir();
#elif ANDROID
            return GetupAndroidLoggingDir();
#else
            throw new InvalidOperationException("OS not configured");
#endif
        }

        private static string GetupWindowsLoggingDir()
        {
            // write logs in the same folder as the app
            //return AppDomain.CurrentDomain.BaseDirectory;
            // write logs to the AppData folder
            return FileSystem.Current.AppDataDirectory;
        }

#if ANDROID
        private static string GetupAndroidLoggingDir()
        {
            var context = Android.App.Application.Context;
            var dirs = context.GetExternalFilesDirs(null);
            if (dirs != null && dirs[0] != null)
            {
                // use our external folder - dependes on package name
                return dirs[0].AbsolutePath;
            }
            else
            {
                // hard code and hope for the best
                return $"/sdcard/Android/data/{context.PackageName}/files/";
            }
        }
#endif

        private void SetupLoggingDir(string folder)
        {
            // set the targets for the file loggers
            var config = LogManager.Configuration;
            var target = config!.FindTargetByName("fileTarget");
            var fileTarget = target as FileTarget;
            if (fileTarget != null)
            {
                fileTarget.FileName = Path.Combine(folder, "logs/worldolio.log");
                fileTarget.ArchiveFileName = Path.Combine(folder, "logs/worldolio.log");
            }
        }

        public Worldolio.Data.Logging.ILogger Logger
        {
            get
            {
                var logger = LogManager.GetCurrentClassLogger();
                return new NLogLogger(logger);
            }
        }
    }
}
