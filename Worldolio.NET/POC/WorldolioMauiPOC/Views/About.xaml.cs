using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.About;

namespace WorldolioMauiPOC.Views;

public partial class About : ContentPage
{
    private ILogger _logger;

    public About(
        AboutViewModel viewModel,
        IToolbarHelper toolbarHelper,
        ILogger logger)
    {
        logger.Debug(() => $"About init");
        BindingContext = viewModel;

        InitializeComponent();

        this.ToolbarItems.Add(toolbarHelper.CreateDoneButton());

        _logger = logger;

    }
}