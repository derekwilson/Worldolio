using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using Worldolio.Data.Logging;
using WorldolioMauiPOC.AppSettings;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.ViewModels.Settings
{
    public partial class SettingsViewModel : INotifyPropertyChanged
    {
        public ICommand NavigateBack { get; }
        public ICommand ResetIds { get; }
        public ICommand UpdateIds { get; }

        public string CurrentSettingsCityIds { get; set; } = "";

        private ILogger _logger;
        private INavigationHelper _navigationHelper;
        private IUserSettings _userSettings;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public SettingsViewModel(ILogger logger, INavigationHelper navigationHelper, IUserSettings userSettings)
        {
            logger.Debug(() => $"SettingsViewModel init");

            _logger = logger;
            _navigationHelper = navigationHelper;
            _userSettings = userSettings;

            CurrentSettingsCityIds = String.Join(',', _userSettings.Cities);

            NavigateBack = new Command(async () => await _navigationHelper.ExecuteNavigationAsync(".."));
            ResetIds = new Command(() =>
            {
                _logger.Debug(() => $"ResetIds");
                CurrentSettingsCityIds = String.Join(',', _userSettings.DefaultCities);
                OnPropertyChanged("CurrentSettingsCityIds");
            });
            UpdateIds = new Command(() =>
            {
                _logger.Debug(() => $"UpdateIds {CurrentSettingsCityIds}");
                _userSettings.SetFromString(CurrentSettingsCityIds, true);
                CurrentSettingsCityIds = String.Join(',', _userSettings.Cities);
                OnPropertyChanged("CurrentSettingsCityIds");
            });
        }

        [RelayCommand]
        private async Task InitAsync()
        {
            _logger.Debug(() => $"SettingsViewModel InitAsync");
        }
    }
}
