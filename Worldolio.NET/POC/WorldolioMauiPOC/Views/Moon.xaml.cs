using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.Moon;

namespace WorldolioMauiPOC.Views;

public partial class Moon : ContentPage
{
    private ILogger _logger;
    private MoonViewModel _viewModel;
    private IToolbarHelper _toolbarHelper;

    public Moon(ILogger logger, MoonViewModel viewModel, IToolbarHelper toolbarHelper)
    {
        logger.Debug(() => $"Moon init");

        InitializeComponent();

        BindingContext = viewModel;

        _logger = logger;
        _viewModel = viewModel;
        _toolbarHelper = toolbarHelper;

        foreach (var item in _toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }
    }
}