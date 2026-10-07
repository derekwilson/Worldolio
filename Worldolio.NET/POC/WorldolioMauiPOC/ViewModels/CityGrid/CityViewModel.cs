using System.ComponentModel;
using Worldolio.Data.Model;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.ViewModels.CityGrid
{
    public class CityViewModel : INotifyPropertyChanged
    {
        private Color _homeColour = Application.Current?.Resources.GetResource<Color>("PrimaryExtraLight", Colors.Red)
            ?? Colors.Red
            ;

        public bool IsHome
        {
            get
            {
                if (_homeCity == null || _homeCity.Id != _city.Id)
                {
                    return false;
                }
                return true;
            }
        }

        public Color ItemBackgroundColor
        {
            get
            {
                if (IsHome)
                {
                    // the home city row
                    return _homeColour;
                }

                // most rows will not be home
                if (Application.Current?.RequestedTheme == AppTheme.Dark)
                {
                    return Colors.Black;
                }
                return Colors.White;
            }
        }

        public void OnThemeChanged()
        {
            OnPropertyChanged(nameof(ItemBackgroundColor));
        }

        public string CityName => _city.DisplayName;

        public string CountryName => _city.Country.DisplayName;

        public string CurrentTime
        {
            get
            {
                return _city.GetCurrentTimeFormatted(_now, _homeCity, _inDayTimeFormat);
            }
        }

        public string CurrentDay
        {
            get
            {
                return _city.GetCurrentTimeFormatted(_now, _homeCity, ITimeZone.TimeFormat.DAY_SHORT);
            }
        }

        public string CurrentDayAndTime => $"{CurrentDay} {CurrentTime}";

        public string OffsetToHome
        {
            get
            {
                return _city.GetFormattedOffset(_now, _homeCity);
            }
        }

        public City City
        {
            get
            {
                return _city;
            }
        }

        public string DSTDates => _city?.TimeZone?.GetDSTDatesForDisplay(_now) ?? "UNKNOWN";

        public string Sunrise => _city.GetSunrise(_now, _inDayTimeFormat);
        public string Noon => _city.GetNoon(_now, _inDayTimeFormat);
        public string Sunset => _city.GetSunset(_now, _inDayTimeFormat);
        public string Moonrise => _city.GetMoonrise(_now, _withDayTimeFormat);
        public string Moonset => _city.GetMoonset(_now, _withDayTimeFormat);

        private DateTime _now = DateTime.MinValue;
        private ITimeZone.TimeFormat _inDayTimeFormat = ITimeZone.TimeFormat.TIME_SHORT_AMPM;
        private ITimeZone.TimeFormat _withDayTimeFormat = ITimeZone.TimeFormat.DAY_TIME_SHORT_AMPM;
        private City _city;
        private City? _homeCity;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public CityViewModel(City city, City? homeCity)
        {
            _city = city;
            _homeCity = homeCity;
        }

        public void Update(DateTime now, ITimeZone.TimeFormat inDayTimeFormat, ITimeZone.TimeFormat withDayTimeFormat)
        {
            _now = now;
            _inDayTimeFormat = inDayTimeFormat;
            _withDayTimeFormat = withDayTimeFormat;
            OnPropertyChanged(nameof(CurrentTime));
            OnPropertyChanged(nameof(CurrentDay));
            OnPropertyChanged(nameof(CurrentDayAndTime));
            // TODO - actually these only need to be done when the day changes
            OnPropertyChanged(nameof(Sunrise));
            OnPropertyChanged(nameof(Sunset));
            OnPropertyChanged(nameof(Moonrise));
            OnPropertyChanged(nameof(Moonset));
        }
    }
}
