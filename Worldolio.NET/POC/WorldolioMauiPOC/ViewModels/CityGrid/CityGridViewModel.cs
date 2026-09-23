using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.AppSettings;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.ViewModels.CityGrid
{

    public partial class CityGridViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CityViewModel> Cities { get; set; } = new ObservableCollection<CityViewModel>();
        public string CurrentTime { get; set; } = "not set";
        public string MoonPhase { get; set; } = "not set";
        public string NumberOfCities { get; set; } = "not set";

        private ITimeZone.TimeFormat _currentInDayTimeFormat = ITimeZone.TimeFormat.TIME_SHORT_AMPM;            // TODO - read from settings
        private ITimeZone.TimeFormat _currentWithDayTimeFormat = ITimeZone.TimeFormat.DAY_TIME_SHORT_AMPM;      // TODO - read from settings
        private DateTime _currentNow;
        private DateTime _lastRefreshTime = DateTime.MinValue;


        private ILogger _logger;
        private ICityRepository _citiesRepository;
        private INavigationHelper _navigationHelper;
        private IDialogHelper _dialogHelper;
        private ISystemTimeProvider _systemTimeProvider;
        private IUserSettings _userSettings;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public CityGridViewModel(ICityRepository citiesRepository, ILogger logger, INavigationHelper navigationHelper, ISystemTimeProvider systemTimeProvider, IUserSettings userSettings, IDialogHelper dialogHelper)
        {
            logger.Debug(() => $"CityGridViewModel init");

            _logger = logger;
            _citiesRepository = citiesRepository;
            _dialogHelper = dialogHelper;
            _navigationHelper = navigationHelper;
            _systemTimeProvider = systemTimeProvider;
            _userSettings = userSettings;
        }

        public void UpdateCityGrid()
        {
            _currentNow = _systemTimeProvider.Now;
            _logger.Debug(() => $"CityGridViewModel UpdateCityGrid: now = {_currentNow}");
            MoonPhase = GeoCalculator.GetFormattedIlluminatedFractionOfMoon(_currentNow);
            // TODO - actually these only need to be done when the day changes
            OnPropertyChanged(nameof(MoonPhase));

            foreach (CityViewModel cityView in Cities)
            {
                cityView.Update(_currentNow, _currentInDayTimeFormat, _currentWithDayTimeFormat);
            }
            if (Cities.Count > 0 && Cities[0] != null)
            {
                CurrentTime = Cities[0].CurrentTime;
            }
            OnPropertyChanged(nameof(CurrentTime));
        }

        [RelayCommand]
        private async Task InitAsync()
        {
            _logger.Debug(() => $"CityGridViewModel InitAsync, last refresh: {_lastRefreshTime}");

            if (_userSettings.HasBeenUpdatedSince(_lastRefreshTime))
            {
                _logger.Debug(() => $"CityGridViewModel InitAsync - refresh needed");

                var temp = await _citiesRepository.GetByIdsAsync(_userSettings.Cities);
                var home = temp.FirstOrDefault();

                Cities.Clear();
                foreach (City city in temp)
                {
                    Cities.Add(new CityViewModel(city, home, _currentNow, _currentInDayTimeFormat, _currentWithDayTimeFormat));
                }

                NumberOfCities = Cities.Count.ToString();
                OnPropertyChanged(nameof(NumberOfCities));
                _lastRefreshTime = _systemTimeProvider.GetUtcNow();
            }

            UpdateCityGrid();
            _logger.Debug(() => $"CityGridViewModel cities = {Cities.Count}");
        }

        // The [RelayCommand] automatically creates an 'ItemTappedCommand' for the XAML
        [RelayCommand]
        private async Task ItemTappedAsync(CityViewModel selectedItem)
        {
            _logger.Debug(() => $"CityGridViewModel ItemTappedAsync");
            if (selectedItem == null)
            {
                _logger.Warning(() => $"CityGridViewModel ItemTappedAsync - NULL selected item");
                return;
            }

            _logger.Debug(() => $"CityGridViewModel ItemTappedAsync {selectedItem.CityName}");
            await _dialogHelper.ShowAlertAsync("Alert", $"City = {selectedItem.CityName}");
        }
    }
}
