using CommunityToolkit.Maui.Core.Platform;
using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.Settings;

namespace WorldolioMauiPOC.Views;

public partial class Settings : ContentPage
{
    private ILogger _logger;
    private SettingsViewModel _viewModel;
    
	public Settings(
        SettingsViewModel viewModel,
        IToolbarHelper toolbarHelper,
        ILogger logger)
	{
        logger.Debug(() => $"Settings init");
        BindingContext = viewModel;

        InitializeComponent();

        this.ToolbarItems.Add(toolbarHelper.CreateDoneButton());

        _logger = logger;
        _viewModel = viewModel;
    }

    protected override void OnDisappearing()
    {
        _logger.Debug(() => $"Settings OnDisappearing");
        base.OnDisappearing();
        // on android the keyboard does not go away when the screen is closed
        SettingIdsEntry.HideKeyboardAsync(CancellationToken.None);
    }
}