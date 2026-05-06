using NodaTime;

namespace Worldolio.Data.Utility
{
    public interface ISystemTimeProvider
    {
        Instant Now { get; }
        Instant GetUtcInstant(int year, int month, int day, int  hour, int minute, int second);
        ZonedDateTime GetUtcNow();
    }

    public class SystemTimeProvider : ISystemTimeProvider
    {
        public Instant Now
        {
            get
            {
                return SystemClock.Instance.GetCurrentInstant();
            }
        }

        public Instant GetUtcInstant(int year, int month, int day, int hour, int minute, int second)
        {
            return Instant.FromUtc(year, month, day, hour, minute, second);
        }

        public ZonedDateTime GetUtcNow()
        {
            return Now.InUtc();
        }
    }
}
