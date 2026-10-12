using System.Globalization;
using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Data
{
    internal static class DatabaseHelper
    {
        // if these change then you also need to change the BuildInfo target in the project file
        const string RESOURCE_DB_NAME = "worldolio.sqlite";
        const string TIMESTAMP_FORMAT = "yyyy-MM-ddTHH:mm:ssK";

        // this is the timestamp of the sqlite file in the resource bundle
        // we get it as a build task when the app is compiled
        static public string GetDbResourceTimestampUtcAsString()
        {
            return BuildInfo.DbTimestamp;
        }

        static private DateTime GetDbResourceTimestampUtc()
        {
            var str = GetDbResourceTimestampUtcAsString();
            return DateTime.ParseExact(str, TIMESTAMP_FORMAT, CultureInfo.InvariantCulture);
        }

        static public string GetDatabaseFilePath()
        {
#if WINDOWS
            //var dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RESOURCE_DB_NAME);
            var dbFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, RESOURCE_DB_NAME);
#elif ANDROID
            var dbFilePath = Path.Combine(FileSystem.AppDataDirectory, RESOURCE_DB_NAME);
#else
            throw new NotImplementedException("DB path on the OS has not been implemented");
#endif
            return dbFilePath;
        }

        public static void CopyDatabaseToFileSystem(ILogger logger)
        {
            CopyDatabaseToFileSystem(logger, GetDatabaseFilePath());
        }

        // to call asyn method synchronously
        public static void CopyDatabaseToFileSystem(ILogger logger, string targetPathname)
        {
            var task = CopyDatabaseToFileSystemAsync(logger, targetPathname);
            Func<System.Runtime.CompilerServices.TaskAwaiter> getAwaiter = task.GetAwaiter;
            Func<System.Runtime.CompilerServices.TaskAwaiter> result = getAwaiter; // Blocks until the task completes
        }

        public static string GetDbFileCreatedDateTimeUtcAsString()
        {
            var info = new FileInfo(GetDatabaseFilePath());
            return info.LastWriteTimeUtc.ToString(TIMESTAMP_FORMAT);
        }

        private static DateTime GetFileCreatedDateTimeUtc(string filepath)
        {
            var info = new FileInfo(filepath);
            return info.LastWriteTimeUtc;
        }

        //Copying method from Maui File System Helper
        public static async Task CopyDatabaseToFileSystemAsync(ILogger logger, string targetPathname)
        {
            bool resourceDbIsNewer = false;
            var dbExists = File.Exists(targetPathname);
            logger.Debug(() => $"CopyDatabaseToFileSystemAsync DB exists = {dbExists}, {targetPathname}");
            if (dbExists)
            {
                var fsDb = GetFileCreatedDateTimeUtc(targetPathname);
                var resourceDb = GetDbResourceTimestampUtc();
                logger.Debug(() => $"FS DB {fsDb}, Resource DB {resourceDb}");
            }

            // only copy if we need to, the db is read only but we dont want to just copy it every time we start
            if (!dbExists || resourceDbIsNewer)
            {
                using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(RESOURCE_DB_NAME);
                {
                    logger.Debug(() => $"CopyDatabaseToFileSystemAsync copying DB {targetPathname}");
                    using FileStream outputStream = File.Create(targetPathname);
                    await inputStream.CopyToAsync(outputStream);
                    logger.Debug(() => $"CopyDatabaseToFileSystemAsync DB copied {targetPathname}");
                }
            }
        }
    }
}
