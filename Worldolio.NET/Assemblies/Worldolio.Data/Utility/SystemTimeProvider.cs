namespace Worldolio.Data.Utility
{
    public interface ISystemTimeProvider
    {
        DateTime Now { get; }
        DateTime GetUtcNow();
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

        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
