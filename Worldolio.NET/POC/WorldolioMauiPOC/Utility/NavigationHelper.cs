using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Utility
{
    public interface INavigationHelper
    {
        Task ExecuteNavigationAsync(string route);
        Task ExecuteModalNavigationAsync<PAGE>()
            where PAGE : ContentPage;
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
                await Shell.Current.Navigation.PushModalAsync(new NavigationPage(modalPage));
            }
            catch (Exception ex)
            {
                _logger.LogException(() => "ExecuteModalNavigationAsync", ex);
            }
        }
    }
}
