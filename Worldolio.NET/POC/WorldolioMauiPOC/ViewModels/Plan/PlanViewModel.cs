using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;
using Worldolio.Data.Repository;
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
                    UpdateTime();
                }
            }
        }

        public ObservableCollection<CityViewModel> Cities { get; set; } = new ObservableCollection<CityViewModel>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private TimeFormat _currentInDayTimeFormat = TimeFormat.TIME_SHORT_AMPM;            // TODO - read from settings
        private TimeFormat _currentWithDayTimeFormat = TimeFormat.DAY_TIME_SHORT_AMPM;      // TODO - read from settings

        private ILogger _logger;
        private ICityRepository _citiesRepository;
        private IUserSettings _userSettings;

        public PlanViewModel(ILogger logger, ICityRepository citiesRepository, IUserSettings userSettings)
        {
            _logger = logger;
            _citiesRepository = citiesRepository;
            _userSettings = userSettings;
        }

        public void UpdateTimeFromSlider(int value)
        {
            // value is in the range 0..96 - every quater of an hour in the day
            if (value < 1 || value > 95)
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
            UpdateTime();
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

        private void UpdateTime()
        {
            _logger.Debug(() => $"PlanViewModel UpdateTime");
            foreach (CityViewModel cityView in Cities)
            {
                cityView.Update(GetNow(), _currentInDayTimeFormat, _currentWithDayTimeFormat);
            }
        }

        [RelayCommand]
        private async Task InitAsync()
        {
            _logger.Debug(() => $"PlanViewModel InitAsync");

            var temp = await _citiesRepository.GetByIdsAsync(_userSettings.Cities);
            var home = temp.FirstOrDefault();

            Cities.Clear();
            foreach (City city in temp)
            {
                Cities.Add(new CityViewModel(city, home, GetNow(), _currentInDayTimeFormat, _currentWithDayTimeFormat));
            }

            _logger.Debug(() => $"PlanViewModel cities = {Cities.Count}");
        }
    }
}
