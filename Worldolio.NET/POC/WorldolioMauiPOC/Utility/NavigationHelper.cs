using Worldolio.Data.Logging;
using Worldolio.Data.Utility;

namespace WorldolioMauiPOC.Utility
{
    public interface INavigationHelper
    {
        Task<PAGE?> ExecuteModalNavigationAsync<PAGE>(bool animated)
            where PAGE : ContentPage;
        Task<PAGE?> ExecuteModalNavigationWithDebounceAsync<PAGE>(bool animated)
            where PAGE : ContentPage;
        Task ExecuteModalNavigationBackAsync();
        Task ExecuteModalNavigationBackWithDebounceAsync();
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

        public async Task<PAGE?> ExecuteModalNavigationAsync<PAGE>(bool animated)
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
                    return modalPage;
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
            return null;
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

        // on android its possible to tap a button twice before the first tap is handled - not on windows
        public async Task<PAGE?> ExecuteModalNavigationWithDebounceAsync<PAGE>(bool animated)
            where PAGE : ContentPage
        {
            _logger.Debug(() => $"ExecuteModalNavigationWithDebounceAsync {typeof(PAGE).FullName}");
            if (IsDoubleTap())
            {
                _logger.Debug(() => $"ExecuteModalNavigationWithDebounceAsync {typeof(PAGE).FullName} - busy - supressed");
                return null;
            }
            else
            {
                return await ExecuteModalNavigationAsync<PAGE>(animated);
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

        // on android its possible to tap a button twice before the first tap is handled - not on windows
        public async Task ExecuteModalNavigationBackWithDebounceAsync()
        {
            _logger.Debug(() => $"ExecuteModalNavigationBackWithDebounceAsync");
            if (IsDoubleTap())
            {
                _logger.Debug(() => $"ExecuteModalNavigationBackWithDebounceAsync - busy - supressed");
                return;
            }
            else
            {
                await ExecuteModalNavigationBackAsync();
            }
        }
    }
}
