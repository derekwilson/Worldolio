using NodaTime;
using System.ComponentModel;
using Worldolio.Data.Model;
using static Worldolio.Data.Model.TimeZone;

namespace WorldolioMauiPOC.ViewModels.CityGrid
{
    public class CityViewModel : INotifyPropertyChanged
    {
        public string CityName => _city.DisplayName;

        public string CountryName => _city.Country.DisplayName;

        public string CurrentTime => _city.TimeZone.GetFormattedLocalTime(_now, _inDayTimeFormat);
        public string CurrentDay => _city.TimeZone.GetFormattedLocalTime(_now, TimeFormat.DAY_SHORT);
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
                    return _city.TimeZone.GetFormattedOffset(_homeCity.TimeZone);
                }
            }
        }

        public string DSTDates => _city.TimeZone.GetDSTDatesForDisplay();

        public string Sunrise => _city.GetSunrise(_now, _inDayTimeFormat);
        public string Noon => _city.GetNoon(_now, _inDayTimeFormat);
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
            OnPropertyChanged(nameof(CurrentDayAndTime));
            // TODO - actually these only need to be done when the day changes
            OnPropertyChanged(nameof(Sunrise));
            OnPropertyChanged(nameof(Sunset));
            OnPropertyChanged(nameof(Moonrise));
            OnPropertyChanged(nameof(Moonset));
        }
    }
}
