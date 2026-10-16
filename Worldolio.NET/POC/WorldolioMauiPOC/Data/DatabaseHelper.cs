using System.Globalization;
using Worldolio.Data.Logging;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.AppSettings;

namespace WorldolioMauiPOC.Data
{
    public interface IDatabaseHelper
    {
        // this is the timestamp of the sqlite file in the resource bundle
        // we get it as a build task when the app is compiled
        // its the timestamp the DB file was last generated
        string GetDbResourceTimestampUtcAsString();

        // this is the time the DB file was copied into the file system
        string GetDbFileCreatedDateTimeUtcAsString();

        // must call this before accessing the DB - to ensure it is copied
        Task InitAsync();
    }

    public class DatabaseHelper : IDatabaseHelper
    {
        // if these change then you also need to change the BuildInfo target task in the project file
        const string RESOURCE_DB_NAME = "worldolio.sqlite";
        const string TIMESTAMP_FORMAT = "yyyy-MM-ddTHH:mm:ssK";

        private ILogger _logger;
        private IUserSettings _userSettings;

        private bool _doneInit = false;

        public DatabaseHelper(ILogger logger, IUserSettings userSettings)
        {
            logger.Debug(() => $"DatabaseHelper ctor");
            _logger = logger;
            _userSettings = userSettings;
        }

        public string GetDbResourceTimestampUtcAsString()
        {
            return BuildInfo.DbTimestamp;
        }

        public static string GetDatabaseFilePath()
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

        // this is the time the DB file was copied into the file system
        public string GetDbFileCreatedDateTimeUtcAsString()
        {
            var info = new FileInfo(GetDatabaseFilePath());
            return info.LastWriteTimeUtc.ToString(TIMESTAMP_FORMAT);
        }

        public async Task InitAsync()
        {
            _logger.Debug(() => $"DatabaseHelper init");
            if (_doneInit)
            {
                _logger.Debug(() => $"DatabaseHelper already done");
                return;
            }

            // database init
            DapperExtensions.AttachMappers();
            await CopyDatabaseToFileSystemAsync(_logger, GetDatabaseFilePath());
            _doneInit = true;
        }

        private async Task CopyDatabaseToFileSystemAsync(ILogger logger, string targetPathname)
        {
            bool resourceDbIsDifferent = false;
            var dbExists = File.Exists(targetPathname);
            logger.Debug(() => $"CopyDatabaseToFileSystemAsync DB exists = {dbExists}, {targetPathname}");
            if (dbExists)
            {
                var resourceDb = GetDbResourceTimestampUtcAsString();
                var lastCopiedDb = _userSettings.LastDbCopiedTimestamp;
                logger.Debug(() => $"Last Copied DB {lastCopiedDb}, Resource DB {resourceDb}");
                if (resourceDb != lastCopiedDb)
                {
                    logger.Debug(() => $"should copy DB");
                    // if the resource version is different then we want to copy it again
                    // if we downgrade the app we should downgrade the DB
                    resourceDbIsDifferent = true;
                }
            }

            // only copy if we need to, the db is read only but we dont want to just copy it every time we start
            if (!dbExists || resourceDbIsDifferent)
            {
                using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(RESOURCE_DB_NAME);
                {
                    logger.Debug(() => $"CopyDatabaseToFileSystemAsync copying DB {targetPathname}");
                    using FileStream outputStream = File.Create(targetPathname);
                    await inputStream.CopyToAsync(outputStream);
                    _userSettings.LastDbCopiedTimestamp = GetDbResourceTimestampUtcAsString();
                    logger.Debug(() => $"CopyDatabaseToFileSystemAsync DB copied {targetPathname}");
                }
            }
        }
    }
}
