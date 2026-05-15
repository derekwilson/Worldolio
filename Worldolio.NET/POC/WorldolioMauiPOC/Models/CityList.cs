using System.Collections.ObjectModel;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;

namespace WorldolioMauiPOC.Models
{
    internal class CityList
    {
        public ObservableCollection<City> Cities { get; set; } = new ObservableCollection<City>();
        private IConnectionFactory _connectionFactory;
        private ICityRepository _citiesRepository;
        private ISystemTimeProvider _systemTimeProvider;

        public CityList()
        {
            DapperExtensions.AttachMappers();
            _systemTimeProvider = new SystemTimeProvider();
            var timeZoneFactory = new TimeZoneFactory(_systemTimeProvider);
#if WINDOWS
            var dbFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\worldolio.sqlite";
#elif ANDROID
            var databaseName = "worldolio.sqlite";
            var dbFilePath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
#else
            var dbFilePath = "./worldolio.sqlite";
#endif
            _connectionFactory = new LocalFileDbConnectionFactory(dbFilePath);
            _citiesRepository = new CityRepository(_connectionFactory, timeZoneFactory);

            LoadCities().GetAwaiter();
        }

        public async Task InitializeDatabase()
        {
            var databaseName = "worldolio.sqlite";
            var targetPath = Path.Combine(FileSystem.AppDataDirectory, databaseName);

            // Only copy if it doesn't already exist to avoid overwriting user data
            if (!File.Exists(targetPath))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(databaseName);
                using var newStream = File.Create(targetPath);
                await stream.CopyToAsync(newStream);
            }
        }

        public async Task LoadCities()
        {
            // TODO - move to shell
            await InitializeDatabase();

            long[] cityIds = [458, 252, 324, 313, 477, 79, 320, 279, 180, 351];
            var temp = await _citiesRepository.GetByIdsAsync(cityIds);
            foreach (City city in temp)
            {
                Cities.Add(city);
            }
        }
    }
}
