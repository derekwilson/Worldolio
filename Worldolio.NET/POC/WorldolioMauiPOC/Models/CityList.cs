using System.Collections.ObjectModel;
using Worldolio.Data.Model;
using Worldolio.Data.MSSQLite;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.Data;

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

        public async Task LoadCities()
        {
            await DatabaseHelper.CopyDatabaseToFileSystemAsync(DatabaseHelper.GetDatabaseFilePath());

            long[] cityIds = [458, 252, 324, 313, 477, 79, 320, 279, 180, 351, 429, 382];
            var temp = await _citiesRepository.GetByIdsAsync(cityIds);
            foreach (City city in temp)
            {
                Cities.Add(city);
            }
        }
    }
}
