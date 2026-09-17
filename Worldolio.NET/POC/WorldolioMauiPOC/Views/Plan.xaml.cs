using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.Plan;

namespace WorldolioMauiPOC.Views;

public partial class Plan : ContentPage
{
    public Plan(PlanViewModel viewModel, ILogger logger, IToolbarHelper toolbarHelper)
    {
        logger.Debug(() => $"Plan init");

        BindingContext = viewModel;

        InitializeComponent();

        foreach (var item in toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }
    }
}