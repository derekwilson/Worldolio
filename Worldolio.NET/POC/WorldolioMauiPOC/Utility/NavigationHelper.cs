using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Utility
{
    public interface INavigationHelper
    {
        Task ExecuteNavigationAsync(string route);
        Task ExecuteModalNavigationAsync<PAGE>()
            where PAGE : ContentPage;
        Task ExecuteModalNavigationBackAsync();
    }

    public class NavigationHelper : INavigationHelper
    {
        private ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public NavigationHelper(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
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

        public async Task ExecuteModalNavigationAsync<PAGE>()
            where PAGE : ContentPage
        {
            try
            {
                _logger.Debug(() => $"NavigationHelper ExecuteModalNavigationAsync {typeof(PAGE).FullName}");
                // dont forget to register them in the MauiProgram like this
                // builder.Services.AddTransient<About>();
                var modalPage = _serviceProvider.GetRequiredService<PAGE>();
                // Perform your asynchronous call
                var navigator = App.Current?.Windows[0].Page?.Navigation;
                if (navigator != null)
                {
                    await navigator.PushModalAsync(new NavigationPage(modalPage), true);
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

        public async Task ExecuteModalNavigationBackAsync()
        {
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
