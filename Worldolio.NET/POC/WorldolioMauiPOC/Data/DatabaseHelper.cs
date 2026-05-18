using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Data
{
    internal static class DatabaseHelper
    {
        const string RESOURCE_DB_NAME = "worldolio.sqlite";

        static public string GetDatabaseFilePath()
        {
#if WINDOWS
            var dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RESOURCE_DB_NAME);
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

        //to call asyn method synchronously
        public static void CopyDatabaseToFileSystem(ILogger logger, string targetPathname)
        {
            var task = CopyDatabaseToFileSystemAsync(logger, targetPathname);
            Func<System.Runtime.CompilerServices.TaskAwaiter> getAwaiter = task.GetAwaiter;
            Func<System.Runtime.CompilerServices.TaskAwaiter> result = getAwaiter; // Blocks until the task completes
        }

        //Copying method from Maui File System Helper
        public static async Task CopyDatabaseToFileSystemAsync(ILogger logger, string targetPathname)
        {
            var dbExists = File.Exists(targetPathname);
            // Only copy if it doesn't already exist to avoid overwriting user data
            if (!dbExists)
            {
                logger.Debug(() => $"CopyDatabaseToFileSystemAsync DB exists = {dbExists}, {targetPathname}");
                using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(RESOURCE_DB_NAME);
                {
                    using FileStream outputStream = File.Create(targetPathname);
                    await inputStream.CopyToAsync(outputStream);
                    logger.Debug(() => $"CopyDatabaseToFileSystemAsync DB copied {targetPathname}");
                }
            }
        }
    }
}
