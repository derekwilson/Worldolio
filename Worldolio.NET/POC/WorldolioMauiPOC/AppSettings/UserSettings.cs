using Worldolio.Data.Logging;
using Worldolio.Data.Utility;

namespace WorldolioMauiPOC.AppSettings
{
    public interface IUserSettings
    {
        long[] DefaultCities { get; }

        long[] Cities { get; }

        void SetCityIdsFromString(String ids, bool store);

        bool CityIdsHaveBeenUpdatedSince(DateTime time);
        void AddCityId(long id);
        void RemoveCityId(long id);
        void MoveCityIdToTop(long id);

        double WindowWidth { get; set; }
        double WindowHeight { get; set;  }
    }

    public class UserSettings : IUserSettings
    {
        private ILogger _logger;
        private ISystemTimeProvider _timeProvider;

        private const string CITY_IDS_KEY = "city_ids";
        private const string WINDOW_WIDTH_KEY = "window_width";
        private const string WINDOW_HEIGHT_KEY = "window_height";

        private const double DEFAULT_WINDOW_WIDTH = 700;
        private const double DEFAULT_WINDOW_HEIGHT = 700;

        public UserSettings(ILogger logger, ISystemTimeProvider timeProvider)
        {
            _logger = logger;
            _timeProvider = timeProvider;

            logger.Debug(() => $"UserSettings init:");
            LoadWindowDimentions();
            string cityIdsFromPrefs = Preferences.Default.Get(CITY_IDS_KEY, String.Join(',', _defaultcities));
            SetCityIdsFromString(cityIdsFromPrefs, false);
            logger.Debug(() => $"UserSettings init: Window w {_windowWidth}, h {_windowHeight}");
            logger.Debug(() => $"UserSettings init: Ids: [{String.Join(',', _cities)}]");
        }

        //private long[] _defaultcities = [458, 252, 477];
        private long[] _defaultcities = [458, 252, 477, 324, 79, 320, 279];
        private long[] _cities = [];
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private double _windowWidth = DEFAULT_WINDOW_WIDTH;
        private double _windowHeight = DEFAULT_WINDOW_HEIGHT;

        public long[] DefaultCities
        {
            get
            {
                return _defaultcities;
            }
        }

        public long[] Cities
        {
            get
            {
                return _cities;
            }
        }

        public double WindowWidth
        {
            get
            {
                return _windowWidth;
            }
            set
            {
                _windowWidth = value;
                Preferences.Default.Set(WINDOW_WIDTH_KEY, _windowWidth);
            }
        }

        public double WindowHeight
        {
            get
            {
                return _windowHeight;
            }
            set
            {
                _windowHeight = value;
                Preferences.Default.Set(WINDOW_HEIGHT_KEY, _windowHeight);
            }
        }

        private void LoadWindowDimentions()
        {
            _windowWidth = Preferences.Default.Get(WINDOW_WIDTH_KEY, DEFAULT_WINDOW_WIDTH);
            _windowHeight = Preferences.Default.Get(WINDOW_HEIGHT_KEY, DEFAULT_WINDOW_HEIGHT);
        }

        public void SetCityIdsFromString(string ids, bool store)
        {
            _logger.Debug(() => $"UserSettings SetFromString: {ids}");
            var idArray = ids.Split(',');
            if (idArray.Length < 1)
            {
                _logger.Debug(() => $"UserSettings SetFromString: invalid {ids}");
                return;
            }
            List<long> idsAsLong = new List<long>();
            foreach (var id in idArray)
            {
                if (long.TryParse(id, out long result))
                {
                    _logger.Debug(() => $"UserSettings SetFromString: adding {result}");
                    idsAsLong.Add(result);
                }
                else
                {
                    _logger.Debug(() => $"UserSettings SetFromString: cannot parse {id}");
                }
            }
            _lastUpdateTime = _timeProvider.GetUtcNow();
            _logger.Debug(() => $"UserSettings SetFromString: last update {_lastUpdateTime}");
            _cities = idsAsLong.ToArray();
            if (store)
            {
                Preferences.Default.Set(CITY_IDS_KEY, String.Join(',', _cities));
            }
        }

        public bool CityIdsHaveBeenUpdatedSince(DateTime time)
        {
            return _lastUpdateTime > time;
        }

        public void AddCityId(long id)
        {
            _logger.Debug(() => $"UserSettings AddCityId: {id}");
            if (_cities.Contains(id))
            {
                _logger.Debug(() => $"UserSettings AddCityId: {id}, duplicate - ignored");
                return;
            }
            _cities = _cities.Append(id).ToArray();
            _lastUpdateTime = _timeProvider.GetUtcNow();
            _logger.Debug(() => $"UserSettings AddCityId: last update {_lastUpdateTime}");
            Preferences.Default.Set(CITY_IDS_KEY, String.Join(',', _cities));
        }

        public void RemoveCityId(long id)
        {
            _logger.Debug(() => $"UserSettings RemoveCityId: {id}");
            if (_cities.Contains(id))
            {
                _cities = _cities.Where(val => val != id).ToArray();
                _lastUpdateTime = _timeProvider.GetUtcNow();
                _logger.Debug(() => $"UserSettings RemoveCityId: last update {_lastUpdateTime}");
                Preferences.Default.Set(CITY_IDS_KEY, String.Join(',', _cities));
            }
            else
            {
                _logger.Debug(() => $"UserSettings RemoveCityId: {id}, not present - ignored");
            }
        }

        public void MoveCityIdToTop(long id)
        {
            _logger.Debug(() => $"UserSettings MoveCityIdToTop: {id}");
            if (_cities.Contains(id))
            {
                var _citiesWithoutId = _cities.Where(val => val != id).ToArray();
                _cities = [id, .._citiesWithoutId];
                _lastUpdateTime = _timeProvider.GetUtcNow();
                _logger.Debug(() => $"UserSettings MoveCityIdToTop: last update {_lastUpdateTime}");
                Preferences.Default.Set(CITY_IDS_KEY, String.Join(',', _cities));
            }
            else
            {
                _logger.Debug(() => $"UserSettings MoveCityIdToTop: {id}, not present - ignored");
            }
        }
    }
}
