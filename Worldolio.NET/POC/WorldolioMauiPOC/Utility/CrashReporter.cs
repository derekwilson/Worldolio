namespace WorldolioMauiPOC.Utility
{
    public interface ICrashReporter
    {
        void TestReporting();
        void LogNonFatalException(Exception ex);
        void LogNonFatalException(string message, Exception ex);
    }

    // if we need to remove all reporting from the code
    public class NullCrashReporter : ICrashReporter
    {
        public void LogNonFatalException(Exception ex)
        {
        }

        public void LogNonFatalException(string message, Exception ex)
        {
        }

        public void TestReporting()
        {
        }
    }

    public class SentryCrashReporter : ICrashReporter
    {
        public void LogNonFatalException(Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }

        public void LogNonFatalException(string message, Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }

        public void TestReporting()
        {
            throw new NotImplementedException("Test Crash Reporting");
        }
    }
}
