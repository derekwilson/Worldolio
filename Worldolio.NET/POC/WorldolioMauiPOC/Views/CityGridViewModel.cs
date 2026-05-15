using System.Collections.ObjectModel;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using WorldolioMauiPOC.Data;

namespace WorldolioMauiPOC.Views
{
    public class CityGridViewModel
    {
        public ObservableCollection<City> Cities { get; set; } = new ObservableCollection<City>();

        private ICityRepository _citiesRepository;

        public CityGridViewModel(ICityRepository citiesRepository)
        {
            _citiesRepository = citiesRepository;

            LoadCities().GetAwaiter();
        }

        public async Task LoadCities()
        {
            await DatabaseHelper.CopyDatabaseToFileSystemAsync(DatabaseHelper.GetDatabaseFilePath());

            long[] cityIds = [458, 252, 324, 477, 79, 320, 279, 180, 351, 429, 382];
            var temp = await _citiesRepository.GetByIdsAsync(cityIds);
            foreach (City city in temp)
            {
                Cities.Add(city);
            }
        }
    }
}
