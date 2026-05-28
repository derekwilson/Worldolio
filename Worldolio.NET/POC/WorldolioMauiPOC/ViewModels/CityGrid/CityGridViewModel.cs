using CommunityToolkit.Mvvm.Input;
using NodaTime;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.Utility;
using static Worldolio.Data.Model.TimeZone;

namespace WorldolioMauiPOC.ViewModels.CityGrid
{
    public class CityViewModel : INotifyPropertyChanged
    {
        public string CityName => _city.DisplayName;

        public string CountryName => _city.Country.DisplayName;

        public string CurrentTime => _city.TimeZone.GetFormattedLocalTime(_now, _inDayTimeFormat);
        public string CurrentDay => _city.TimeZone.GetFormattedLocalTime(_now, TimeFormat.DAY_SHORT);

        public string OffsetToHome
        {
            get
            {
                if (_homeCity == null)
                {
                    return "";
                }
                else
                {
                    return _city.TimeZone.GetFormattedOffset(_homeCity.TimeZone);
                }
            }
        }

        public string DSTDates => _city.TimeZone.GetDSTDatesForDisplay();

        public string Sunrise => _city.GetSunrise(_now, _inDayTimeFormat);
        public string Sunset => _city.GetSunset(_now, _inDayTimeFormat);
        public string Moonrise => _city.GetMoonrise(_now, _withDayTimeFormat);
        public string Moonset => _city.GetMoonset(_now, _withDayTimeFormat);

        private Instant _now;
        private TimeFormat _inDayTimeFormat;
        private TimeFormat _withDayTimeFormat;
        private City _city;
        private City? _homeCity;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public CityViewModel(City city, City? homeCity, Instant now, TimeFormat inDayTimeFormat, TimeFormat withDayTimeFormat)
        {
            _city = city;
            _homeCity = homeCity;
            _now = now;
            _inDayTimeFormat = inDayTimeFormat;
            _withDayTimeFormat = withDayTimeFormat;
        }

        public void Update(Instant now, TimeFormat inDayTimeFormat, TimeFormat withDayTimeFormat)
        {
            _now = now;
            _inDayTimeFormat = inDayTimeFormat;
            _withDayTimeFormat = withDayTimeFormat;
            OnPropertyChanged(nameof(CurrentTime));
            OnPropertyChanged(nameof(CurrentDay));
            // TODO - actually these only need to be done when the day changes
            OnPropertyChanged(nameof(Sunrise));
            OnPropertyChanged(nameof(Sunset));
            OnPropertyChanged(nameof(Moonrise));
            OnPropertyChanged(nameof(Moonset));
        }
    }

    public partial class CityGridViewModel : INotifyPropertyChanged
    {
        public ICommand NavigateToAboutPage { get; }

        public ObservableCollection<CityViewModel> Cities { get; set; } = new ObservableCollection<CityViewModel>();
        public string CurrentTime { get; set; } = "not set";
        public string MoonPhase { get; set; } = "not set";
        public string NumberOfCities { get; set; } = "not set";


        private TimeFormat _currentInDayTimeFormat = TimeFormat.TIME_SHORT_AMPM;            // TODO - read from settings
        private TimeFormat _currentWithDayTimeFormat = TimeFormat.DAY_TIME_SHORT_AMPM;      // TODO - read from settings
        private Instant _currentInstant;
        private Timer _timer;

        private ILogger _logger;
        private ICityRepository _citiesRepository;
        private INavigationHelper _navigationHelper;
        private ISystemTimeProvider _systemTimeProvider;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public CityGridViewModel(ICityRepository citiesRepository, ILogger logger, INavigationHelper navigationHelper, ISystemTimeProvider systemTimeProvider)
        {
            logger.Debug(() => $"CityGridViewModel init");

            _logger = logger;
            _citiesRepository = citiesRepository;
            _navigationHelper = navigationHelper;
            _systemTimeProvider = systemTimeProvider;

            //NavigateToAboutPage = new Command(async () => await _navigationHelper.ExecuteNavigationAsync(nameof(About)));
            NavigateToAboutPage = new Command(async () => await _navigationHelper.ExecuteModalNavigationAsync<Views.About>());

            // Initialize timer to fire immediately, then tick every 1 second
            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        }

        ~CityGridViewModel() {
            _timer?.Dispose();
        }

        private void TimerCallback(object? state)
        {
            // MainThread required since Timer ticks on a background thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _logger.Debug(() => $"CityGridViewModel TimerCallback");
                UpdateTime();
                _logger.Debug(() => $"CityGridViewModel TimerCallback - end");
            });
        }

        private void UpdateTime()
        {
            _currentInstant = _systemTimeProvider.Now;
            // maybe we should be using the home city?
            DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            ZonedDateTime zdt = _currentInstant.InZone(tz);
            CurrentTime = Worldolio.Data.Model.TimeZone.FormatTime(_currentInDayTimeFormat, zdt.LocalDateTime);
            MoonPhase = GeoCalculator.GetFormattedIlluminatedFractionOfMoon(_currentInstant);

            _logger.Debug(() => $"CityGridViewModel UpdateTime: {CurrentTime}");
            OnPropertyChanged(nameof(CurrentTime));
            // TODO - actually these only need to be done when the day changes
            OnPropertyChanged(nameof(MoonPhase));

            foreach (CityViewModel cityView in Cities)
            {
                cityView.Update(_currentInstant, _currentInDayTimeFormat, _currentWithDayTimeFormat);
            }
        }

        [RelayCommand]
        private async Task InitAsync()
        {
            _logger.Debug(() => $"CityGridViewModel InitAsync");

            long[] cityIds = [458, 252, 324, 477, 79, 320, 279, 180, 351, 429, 382];
            var temp = await _citiesRepository.GetByIdsAsync(cityIds);
            var home = temp.FirstOrDefault();

            Cities.Clear();
            foreach (City city in temp)
            {
                Cities.Add(new CityViewModel(city, home, _currentInstant, _currentInDayTimeFormat, _currentWithDayTimeFormat));
            }

            NumberOfCities = Cities.Count.ToString();
            OnPropertyChanged(nameof(NumberOfCities));

            _logger.Debug(() => $"CityGridViewModel cities = {Cities.Count}");
        }
    }
}
