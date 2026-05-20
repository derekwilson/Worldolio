using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Utility
{
    public interface INavigationHelper
    {
        Task ExecuteNavigationAsync(string route);
    }

    public class NavigationHelper : INavigationHelper
    {
        private ILogger _logger;

        public NavigationHelper(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ExecuteNavigationAsync(string route)
        {
            try
            {
                _logger.Debug(() => $"NavigationHelper ExecuteNavigationAsync {route}");
                // Perform your asynchronous call
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                _logger.LogException(() => "ExecuteNavigationAsync", ex);
            }
        }
    }
}
