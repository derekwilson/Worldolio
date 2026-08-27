using Worldolio.Data.Logging;
using WorldolioMauiPOC.ViewModels.Settings;

namespace WorldolioMauiPOC.Views;

public partial class Settings : ContentPage
{
    private ILogger _logger;
    
	public Settings(SettingsViewModel viewModel, ILogger logger)
	{
        logger.Debug(() => $"Settings init");
        InitializeComponent();

        _logger = logger;

        BindingContext = viewModel;
    }
}