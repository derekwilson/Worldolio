using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.Plan;

namespace WorldolioMauiPOC.Views;

public partial class Plan : ContentPage
{
    private ILogger _logger;
    private PlanViewModel _viewModel;
    private IToolbarHelper _toolbarHelper;

    public Plan(PlanViewModel viewModel, ILogger logger, IToolbarHelper toolbarHelper)
    {
        logger.Debug(() => $"Plan init");
        _logger = logger;
        _viewModel = viewModel;
        _toolbarHelper = toolbarHelper;

        BindingContext = viewModel;

        InitializeComponent();

        foreach (var item in _toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }
    }
}