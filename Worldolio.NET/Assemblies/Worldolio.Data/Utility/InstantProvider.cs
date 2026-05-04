using NodaTime;

namespace Worldolio.Data.Utility
{
    public interface IInstantProvider
    {
        Instant Now { get; }
        Instant GetUtcInstant(int year, int month, int day, int  hour, int minute, int second);
    }

    public class InstantProvider : IInstantProvider
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
    }
}
