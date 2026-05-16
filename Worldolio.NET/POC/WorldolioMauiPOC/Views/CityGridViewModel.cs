using System.Collections.ObjectModel;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.Views
{
    public class CityGridViewModel
    {
        public ObservableCollection<City> Cities { get; set; } = new ObservableCollection<City>();

        public string AppVersion { get; set; }

        private ILogger _logger;
        private ICityRepository _citiesRepository;
        private IEnvironmentInformationProvider _environmentInformationProvider;

        public CityGridViewModel(ICityRepository citiesRepository, ILogger logger, IEnvironmentInformationProvider environmentInformationProvider)
        {
            _logger = logger;
            _citiesRepository = citiesRepository;
            _environmentInformationProvider = environmentInformationProvider;

            _logger.Debug(() => $"CityGridViewModel init");

            AppVersion = _environmentInformationProvider.GetAppVersion();

            LoadCities().GetAwaiter();
        }

        public async Task LoadCities()
        {
            _logger.Debug(() => $"CityGridViewModel LoadCities");

            long[] cityIds = [458, 252, 324, 477, 79, 320, 279, 180, 351, 429, 382];
            var temp = await _citiesRepository.GetByIdsAsync(cityIds);
            foreach (City city in temp)
            {
                Cities.Add(city);
            }

            _logger.Debug(() => $"CityGridViewModel cities = {Cities.Count}");
        }
    }
}
