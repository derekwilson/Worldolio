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

        public static void CopyDatabaseToFileSystem()
        {
            CopyDatabaseToFileSystem(GetDatabaseFilePath());
        }

        //to call asyn method synchronously
        public static void CopyDatabaseToFileSystem(string targetPathname)
        {
            var task = CopyDatabaseToFileSystemAsync(targetPathname);
            Func<System.Runtime.CompilerServices.TaskAwaiter> getAwaiter = task.GetAwaiter;
            Func<System.Runtime.CompilerServices.TaskAwaiter> result = getAwaiter; // Blocks until the task completes
        }

        //Copying method from Maui File System Helper
        public static async Task CopyDatabaseToFileSystemAsync(string targetPathname)
        {
            // Only copy if it doesn't already exist to avoid overwriting user data
            if (!File.Exists(targetPathname))
            {
                using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(RESOURCE_DB_NAME);
                {
                    // Copy the file to the AppDataDirectory
                    using FileStream outputStream = File.Create(targetPathname);
                    await inputStream.CopyToAsync(outputStream);
                }
            }
        }

        public static async Task InitializeDatabase()
        {
            var databaseName = "worldolio.sqlite";
            //var targetPath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
            var targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, databaseName);

            // Only copy if it doesn't already exist to avoid overwriting user data
            if (!File.Exists(targetPath))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(databaseName);
                using var newStream = File.Create(targetPath);
                await stream.CopyToAsync(newStream);
            }
        }
    }
}
