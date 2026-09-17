using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.AppSettings;
using WorldolioMauiPOC.ViewModels.CityGrid;
using static Worldolio.Data.Model.TimeZone;

namespace WorldolioMauiPOC.ViewModels.Plan
{
    public partial class PlanViewModel : INotifyPropertyChanged
    {
        public int CurrentHour { get; set; } = 0;
        public int CurrentMinute { get; set; } = 0;

        public string CurrentTime
        {
            get
            {
                return $"{CurrentHour}:{CurrentMinute:00}";
            }
        }

        private DateTime _selectedDate = DateTime.Today;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged("SelectedDate");
                    UpdateTimeInGrid();
                }
            }
        }

        public int SliderMin
        {
            get
            {
                return 0;
            }
        }

        public int SliderMax
        {
            get
            {
                return 95;
            }
        }

        // The value we want the slider to increment each time it updates
        private readonly int _sliderIncrement = 1;
        // The hardwired default is 12 noon
        private int _currentSliderValue = 48;
        public double SliderValue
        {
            get => _currentSliderValue;
            set
            {
                if (_currentSliderValue != value)
                {
                    var sliderCorrectValue = (int)(value / _sliderIncrement) * _sliderIncrement;
                    _logger.Debug(() => $"SliderValue set: {_currentSliderValue} -> {value}, {sliderCorrectValue}");
                    if (sliderCorrectValue != _currentSliderValue)
                    {
                        _currentSliderValue = sliderCorrectValue;
                        OnPropertyChanged("SliderValue");
                        UpdateTimeFromSlider(_currentSliderValue);
                    }
                }
            }
        }

        public ObservableCollection<CityViewModel> Cities { get; set; } = new ObservableCollection<CityViewModel>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private ITimeZone.TimeFormat _currentInDayTimeFormat = ITimeZone.TimeFormat.TIME_SHORT_AMPM;            // TODO - read from settings
        private ITimeZone.TimeFormat _currentWithDayTimeFormat = ITimeZone.TimeFormat.DAY_TIME_SHORT_AMPM;      // TODO - read from settings
        private DateTime _lastRefreshTime = DateTime.MinValue;

        private ILogger _logger;
        private ICityRepository _citiesRepository;
        private ISystemTimeProvider _systemTimeProvider;
        private IUserSettings _userSettings;

        public PlanViewModel(ILogger logger, ICityRepository citiesRepository, IUserSettings userSettings, ISystemTimeProvider systemTimeProvider)
        {
            _logger = logger;
            _citiesRepository = citiesRepository;
            _userSettings = userSettings;
            _systemTimeProvider = systemTimeProvider;

            _selectedDate = _systemTimeProvider.GetToday();
        }

        public void UpdateTimeFromSlider(int value)
        {
            // value is in the range 0..95 - every quater of an hour in the day
            if (value < 1 || value > SliderMax)
            {
                CurrentHour = 0;
                CurrentMinute = 0;
            }
            else
            {
                CurrentHour = value / 4;
                CurrentMinute = (value % 4) * 15;
            }
            _logger.Debug(() => $"UpdateTimeFromSlider: {value} -> {CurrentHour}, {CurrentMinute}");
            OnPropertyChanged("CurrentHour");
            OnPropertyChanged("CurrentMinute");
            OnPropertyChanged("CurrentTime");
            UpdateTimeInGrid();
        }

        private DateTime GetNow()
        {
            if (Cities.Count < 1)
            {
                //throw new InvalidOperationException("no home city");
            }
            DateTime dt = new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day, CurrentHour, CurrentMinute, 0);
            DateTime dtUtc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return dtUtc;
        }

        private void UpdateTimeInGrid()
        {
            _logger.Debug(() => $"PlanViewModel UpdateTimeInGrid");
            foreach (CityViewModel cityView in Cities)
            {
                cityView.Update(GetNow(), _currentInDayTimeFormat, _currentWithDayTimeFormat);
            }
        }

        [RelayCommand]
        private async Task InitAsync()
        {
            _logger.Debug(() => $"PlanViewModel InitAsync, last refresh: {_lastRefreshTime}");

            if (_userSettings.HasBeenUpdatedSince(_lastRefreshTime))
            {
                _logger.Debug(() => $"PlanViewModel InitAsync - refresh needed");

                var temp = await _citiesRepository.GetByIdsAsync(_userSettings.Cities);
                var home = temp.FirstOrDefault();

                Cities.Clear();
                foreach (City city in temp)
                {
                    Cities.Add(new CityViewModel(city, home, GetNow(), _currentInDayTimeFormat, _currentWithDayTimeFormat));
                }

                _lastRefreshTime = _systemTimeProvider.GetUtcNow();
                UpdateTimeFromSlider(_currentSliderValue);
            }

            _logger.Debug(() => $"PlanViewModel cities = {Cities.Count}");
        }

        [RelayCommand]
        public void LeftClicked()
        {
            _logger.Debug(() => $"PlanViewModel LeftClicked current = {_currentSliderValue}");
            if (_currentSliderValue > SliderMin)
            {
                SliderValue = _currentSliderValue - _sliderIncrement;
            }
        }

        [RelayCommand]
        public void RightClicked()
        {
            _logger.Debug(() => $"PlanViewModel RightClicked current = {_currentSliderValue}");
            if (_currentSliderValue < SliderMax)
            {
                SliderValue = _currentSliderValue + _sliderIncrement;
            }
        }
    }
}