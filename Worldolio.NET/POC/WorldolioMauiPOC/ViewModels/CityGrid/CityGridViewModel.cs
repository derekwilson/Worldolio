using System.Collections.ObjectModel;
using System.Windows.Input;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.ViewModels.CityGrid
{
    public class CityGridViewModel
    {
        public ICommand NavigateToAboutPage { get; }

        public ObservableCollection<City> Cities { get; set; } = new ObservableCollection<City>();

        public string AppVersion { get; set; }

        private ILogger _logger;
        private ICityRepository _citiesRepository;
        private IEnvironmentInformationProvider _environmentInformationProvider;
        private INavigationHelper _navigationHelper;

        public CityGridViewModel(ICityRepository citiesRepository, ILogger logger, IEnvironmentInformationProvider environmentInformationProvider, INavigationHelper navigationHelper)
        {
            logger.Debug(() => $"CityGridViewModel init");

            _logger = logger;
            _citiesRepository = citiesRepository;
            _environmentInformationProvider = environmentInformationProvider;
            _navigationHelper = navigationHelper;

            NavigateToAboutPage = new Command(async () => await _navigationHelper.ExecuteNavigationAsync(nameof(About)));

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
