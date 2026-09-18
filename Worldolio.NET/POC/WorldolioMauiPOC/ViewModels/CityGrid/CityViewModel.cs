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

        public Color ItemBackgroundColor
        {
            get
            {
                if (_homeCity == null || _homeCity.Id != _city.Id)
                {
                    // most rows
                    if (Application.Current?.RequestedTheme == AppTheme.Dark)
                    {
                        return Colors.Black;
                    }
                    return Colors.White;
                }
                else
                {
                    // the home city row
                    return _homeColour;
                }
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
                if (_homeCity == null)
                {
                    // maybe revert to the system TZ
                    return "UNKNOWN";
                }
                else
                {
                    return _city.TimeZone.ToLocalTimeFormatted(_now, _homeCity.TimeZone, _inDayTimeFormat);
                }
            }
        }

        public string CurrentDay
        {
            get
            {
                if (_homeCity == null)
                {
                    // maybe revert to the system TZ
                    return "UNKNOWN";
                }
                else
                {
                    return _city.TimeZone.ToLocalTimeFormatted(_now, _homeCity.TimeZone, ITimeZone.TimeFormat.DAY_SHORT);
                }
            }
        }

        public string CurrentDayAndTime => $"{CurrentDay} {CurrentTime}";

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
                    return _city.TimeZone.GetFormattedOffset(_now, _homeCity.TimeZone);
                }
            }
        }

        public string DSTDates => _city.TimeZone.GetDSTDatesForDisplay(_now);

        public string Sunrise => _city.GetSunrise(_now, _inDayTimeFormat);
        public string Noon => _city.GetNoon(_now, _inDayTimeFormat);
        public string Sunset => _city.GetSunset(_now, _inDayTimeFormat);
        public string Moonrise => _city.GetMoonrise(_now, _withDayTimeFormat);
        public string Moonset => _city.GetMoonset(_now, _withDayTimeFormat);

        private DateTime _now;
        private ITimeZone.TimeFormat _inDayTimeFormat;
        private ITimeZone.TimeFormat _withDayTimeFormat;
        private City _city;
        private City? _homeCity;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public CityViewModel(City city, City? homeCity, DateTime now, ITimeZone.TimeFormat inDayTimeFormat, ITimeZone.TimeFormat withDayTimeFormat)
        {
            _city = city;
            _homeCity = homeCity;
            _now = now;
            _inDayTimeFormat = inDayTimeFormat;
            _withDayTimeFormat = withDayTimeFormat;
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
