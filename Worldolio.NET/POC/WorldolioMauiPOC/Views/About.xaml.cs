using Worldolio.Data.Logging;
using WorldolioMauiPOC.ViewModels.About;

namespace WorldolioMauiPOC.Views;

public partial class About : ContentPage
{
    private ILogger _logger;

    public About(AboutViewModel viewModel, ILogger logger)
    {
        logger.Debug(() => $"About init");
        InitializeComponent();

        _logger = logger;

#if WINDOWS
        AddBackButtonToToolbar(viewModel);
#endif
        BindingContext = viewModel;
    }

    private void AddBackButtonToToolbar(AboutViewModel viewModel)
    {
        var backButtonToolbarItem = new ToolbarItem()
        {
            Text = "Done",
            IconImageSource = "arrow_back.png",
            Order = ToolbarItemOrder.Primary,
            Priority = 0,
            Command = viewModel.NavigateBack
        };
        ToolbarItems.Insert(0, backButtonToolbarItem);
        _logger.Debug(() => $"About Added back button to toolbar");
    }
}