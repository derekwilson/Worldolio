namespace Worldolio.Data.Utility
{
    public interface ISystemTimeProvider
    {
        DateTime Now { get; }
        DateTime GetUtcNow();
        DateTime GetToday();
    }

    public class SystemTimeProvider : ISystemTimeProvider
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        public DateTime GetToday()
        {
            return DateTime.Today;
        }

        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
