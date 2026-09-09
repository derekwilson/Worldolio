using Worldolio.Data.Logging;
using Worldolio.Data.Utility;

namespace WorldolioMauiPOC.Utility
{
    public interface INavigationHelper
    {
        Task ExecuteNavigationAsync(string route);
        Task ExecuteModalNavigationAsync<PAGE>(bool animated)
            where PAGE : ContentPage;
        Task ExecuteModalNavigationWithDebounceAsync<PAGE>(bool animated)
            where PAGE : ContentPage;
        Task ExecuteModalNavigationBackAsync();
    }

    public class NavigationHelper : INavigationHelper
    {
        private ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private ISystemTimeProvider _systemTimeProvider;

        public NavigationHelper(ILogger logger, IServiceProvider serviceProvider, ISystemTimeProvider systemTimeProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _systemTimeProvider = systemTimeProvider;
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

        public async Task ExecuteModalNavigationAsync<PAGE>(bool animated)
            where PAGE : ContentPage
        {
            try
            {
                _logger.Debug(() => $"NavigationHelper ExecuteModalNavigationAsync ({animated}) {typeof(PAGE).FullName}");
                // dont forget to register them in the MauiProgram like this
                // builder.Services.AddTransient<About>();
                var modalPage = _serviceProvider.GetRequiredService<PAGE>();
                // Perform your asynchronous call
                var navigator = App.Current?.Windows[0].Page?.Navigation;
                if (navigator != null)
                {
                    await navigator.PushModalAsync(new NavigationPage(modalPage), animated);
                } 
                else
                {
                    _logger.Warning(() => $"NavigationHelper ExecuteModalNavigationAsync - NULL navigator {typeof(PAGE).FullName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(() => "ExecuteModalNavigationAsync", ex);
            }
        }

        private DateTime _lastClick = DateTime.MinValue;

        private bool IsDoubleTap(int thresholdMs = 1000)
        {
            var now = _systemTimeProvider.GetUtcNow();
            if ((now - _lastClick).TotalMilliseconds < thresholdMs)
            {
                return true;
            }
            _lastClick = now;
            return false;
        }

        public async Task ExecuteModalNavigationWithDebounceAsync<PAGE>(bool animated)
            where PAGE : ContentPage
        {
            _logger.Debug(() => $"ExecuteModalNavigationWithDebounceAsync {typeof(PAGE).FullName}");
            if (IsDoubleTap())
            {
                _logger.Debug(() => $"ExecuteModalNavigationWithDebounceAsync {typeof(PAGE).FullName} - busy - supressed");
                return;
            }
            else
            {
                await ExecuteModalNavigationAsync<PAGE>(animated);
            }
        }

        public async Task ExecuteModalNavigationBackAsync()
        {
            _logger.Debug(() => $"NavigationHelper ExecuteModalNavigationBackAsync");
            var navigator = App.Current?.Windows[0].Page?.Navigation;
            if (navigator != null)
            {
                await navigator.PopModalAsync(true);
            }
            else
            {
                _logger.Warning(() => $"NavigationHelper ExecuteModalNavigationBackAsync - NULL navigator");
            }
        }
    }
}
