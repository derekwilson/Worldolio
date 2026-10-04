using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.AppSettings;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.CityGrid;

namespace WorldolioMauiPOC.ViewModels.Settings
{
    public partial class SettingsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<City> Cities { get; set; } = new ObservableCollection<City>();
        private DateTime _lastRefreshTime = DateTime.MinValue;


        public ICommand ResetIds { get; }
        public ICommand UpdateIds { get; }

        public string CurrentSettingsCityIds { get; set; } = "";

        private ILogger _logger;
        private IUserSettings _userSettings;
        private ISystemTimeProvider _systemTimeProvider;
        private ICityRepository _citiesRepository;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public SettingsViewModel(
            ILogger logger,
            IUserSettings userSettings,
            ISystemTimeProvider systemTimeProvider,
            ICityRepository citiesRepository)
        {
            logger.Debug(() => $"SettingsViewModel init");

            _logger = logger;
            _userSettings = userSettings;
            _systemTimeProvider = systemTimeProvider;
            _citiesRepository = citiesRepository;

            CurrentSettingsCityIds = String.Join(',', _userSettings.Cities);

            ResetIds = new Command(() =>
            {
                _logger.Debug(() => $"ResetIds");
                CurrentSettingsCityIds = String.Join(',', _userSettings.DefaultCities);
                OnPropertyChanged("CurrentSettingsCityIds");
            });
            UpdateIds = new Command(() =>
            {
                _logger.Debug(() => $"UpdateIds {CurrentSettingsCityIds}");
                _userSettings.SetCityIdsFromString(CurrentSettingsCityIds, true);
                CurrentSettingsCityIds = String.Join(',', _userSettings.Cities);
                OnPropertyChanged("CurrentSettingsCityIds");
            });
        }

        [RelayCommand]
        private async Task InitAsync()
        {
            _logger.Debug(() => $"SettingsViewModel InitAsync, last refresh: {_lastRefreshTime}");

            if (_userSettings.CityIdsHaveBeenUpdatedSince(_lastRefreshTime))
            {
                _logger.Debug(() => $"SettingsViewModel InitAsync - refresh needed");

                var temp = await _citiesRepository.GetByIdsAsync(_userSettings.Cities);
                Cities = new ObservableCollection<City>(temp);

                OnPropertyChanged(nameof(Cities));

                _lastRefreshTime = _systemTimeProvider.GetUtcNow();
            }

            _logger.Debug(() => $"SettingsViewModel cities = {Cities.Count}");
        }
    }
}
