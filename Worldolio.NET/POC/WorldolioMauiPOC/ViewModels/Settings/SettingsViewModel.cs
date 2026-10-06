using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.AppSettings;

namespace WorldolioMauiPOC.ViewModels.Settings
{
    public partial class SettingsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<City> Cities { get; set; } = new ObservableCollection<City>();
        public string CurrentSettingsCityIds { get; set; } = "";
        private DateTime _lastRefreshTime = DateTime.MinValue;
        public bool IsBusy { get; set; } = false;


        public ICommand ResetIds { get; }
        public ICommand UpdateIds { get; }

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
                InitAsync();
            });
        }

        [RelayCommand]
        private async Task InitAsync()
        {
            _logger.Debug(() => $"SettingsViewModel InitAsync, Busy: {IsBusy}");
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                OnPropertyChanged(nameof(IsBusy));

                // This work runs asynchronously, keeping the UI responsive
                await Task.Run(async () =>
                {
                    await LoadDataIfNeeded();
                });
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsBusy));
                _logger.Debug(() => $"SettingsViewModel InitAsync, Finally Busy: {IsBusy}");
            }

            _logger.Debug(() => $"SettingsViewModel InitAsync, Done");
        }

        private async Task LoadDataIfNeeded()
        {
            _logger.Debug(() => $"SettingsViewModel LoadDataIfNeeded, last refresh: {_lastRefreshTime}");

            if (_userSettings.CityIdsHaveBeenUpdatedSince(_lastRefreshTime))
            {
                _logger.Debug(() => $"SettingsViewModel LoadDataIfNeeded - refresh needed");

                CurrentSettingsCityIds = String.Join(',', _userSettings.Cities);

                var temp = await _citiesRepository.GetByIdsAsync(_userSettings.Cities);
                Cities = new ObservableCollection<City>(temp);

                OnPropertyChanged(nameof(CurrentSettingsCityIds));
                OnPropertyChanged(nameof(Cities));

                _lastRefreshTime = _systemTimeProvider.GetUtcNow();
            }

            _logger.Debug(() => $"SettingsViewModel LoadDataIfNeeded cities = {Cities.Count}");
        }

        // The [RelayCommand] automatically creates an 'DeleteItemCommand' for the XAML
        [RelayCommand]
        private async Task DeleteItemAsync(City selectedCity)
        {
            _logger.Debug(() => $"SettingsViewModel DeleteItemAsync");
            if (selectedCity == null)
            {
                _logger.Warning(() => $"SettingsViewModel DeleteItemAsync - NULL selected item");
                return;
            }

            _logger.Debug(() => $"SettingsViewModel DeleteItemAsync {selectedCity.DisplayName}");
            _userSettings.RemoveCityId(selectedCity.Id);
            await InitAsync();
        }

    }
}
