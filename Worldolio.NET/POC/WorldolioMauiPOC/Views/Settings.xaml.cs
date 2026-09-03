using Worldolio.Data.Logging;
using WorldolioMauiPOC.ViewModels.Settings;
using CommunityToolkit.Maui.Core.Platform;

namespace WorldolioMauiPOC.Views;

public partial class Settings : ContentPage
{
    private ILogger _logger;
    private SettingsViewModel _viewModel;
    
	public Settings(SettingsViewModel viewModel, ILogger logger)
	{
        logger.Debug(() => $"Settings init");
        InitializeComponent();

        _logger = logger;
        _viewModel = viewModel;

        BindingContext = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // on android the keyboard does not go away when the screen is closed
        SettingIdsEntry.HideKeyboardAsync(CancellationToken.None);
    }
}