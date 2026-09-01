using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.AppSettings
{
    public interface IUserSettings
    {
        long[] DefaultCities { get; set; }

        long[] Cities { get; set; }

        void SetFromString(String ids, bool store);
    }

    public class UserSettings : IUserSettings
    {
        private ILogger _logger;

        private const string CITY_IDS_KEY = "city_ids";

        public UserSettings(ILogger logger)
        {
            _logger = logger;

            logger.Debug(() => $"UserSettings init:");
            string cityIdsFromPrefs = Preferences.Default.Get(CITY_IDS_KEY, String.Join(',', _defaultcities));
            SetFromString(cityIdsFromPrefs, false);
            logger.Debug(() => $"UserSettings init: [{String.Join(',',_cities)}]");
        }

        //private long[] _defaultcities = [458, 252, 477];
        private long[] _defaultcities = [458, 252, 477, 324, 79, 320, 279];
        private long[] _cities = [];

        public long[] DefaultCities
        {
            get
            {
                return _defaultcities;
            }
            set
            {
                _defaultcities = value;
            }
        }

        public long[] Cities
        {
            get
            {
                return _cities;
            }
            set
            {
                _cities = value;
            }
        }

        public void SetFromString(string ids, bool store)
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
            _cities = idsAsLong.ToArray();
            if (store)
            {
                Preferences.Default.Set(CITY_IDS_KEY, String.Join(',', _cities));
            }
        }
    }
}
