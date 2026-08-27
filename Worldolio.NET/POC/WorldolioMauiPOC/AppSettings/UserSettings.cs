namespace WorldolioMauiPOC.AppSettings
{
    public interface IUserSettings
    {
        long[] Cities { get; set; }
    }
    public class UserSettings : IUserSettings
    {
        private long[] _cities = [458, 252, 477, 324, 79, 320, 279, 429, 180, 351];

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
    }
}
