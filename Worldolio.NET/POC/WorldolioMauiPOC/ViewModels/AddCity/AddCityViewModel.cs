using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using WorldolioMauiPOC.AppSettings;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.ViewModels.AddCity
{
    public partial class AddCityViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<City> Cities { get; set; } = new ObservableCollection<City>();
        public bool IsAddEnabled
        { 
            get
            {
                return _selectedCity != null;
            }
        }

        private City? _selectedCity = null;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private ILogger _logger;
        private IUserSettings _userSettings;
        private INavigationHelper _navigationHelper;

        public AddCityViewModel(ILogger logger, IUserSettings userSettings, INavigationHelper navigationHelper)
        {
            logger.Debug(() => $"AddCityViewModel init");

            _logger = logger;
            _userSettings = userSettings;
            _navigationHelper = navigationHelper;
        }

        public void SetCities(ICollection<City> cities)
        {
            _logger.Debug(() => $"AddCityViewModel SetCities");
            Cities = new ObservableCollection<City>(cities);
            _selectedCity = null;

            OnPropertyChanged(nameof(Cities));
            OnPropertyChanged(nameof(IsAddEnabled));
        }

        [RelayCommand]
        private void ItemSelected(City selectedItem)
        {
            if (selectedItem == null)
                return;

            _logger.Debug(() => $"AddCityViewModel ItemSelected {selectedItem.DisplayName}");
            _selectedCity = selectedItem;
            OnPropertyChanged(nameof(IsAddEnabled));
        }

        [RelayCommand]
        private async Task AddSelectedAsync()
        {
            _logger.Debug(() => $"AddCityViewModel AddSelectedAsync {_selectedCity?.DisplayName}");
            if (_selectedCity != null)
            {
                _userSettings.AddCityId(_selectedCity.Id);
                await _navigationHelper.ExecuteModalNavigationBackWithDebounceAsync();
            }
        }
    }
}
