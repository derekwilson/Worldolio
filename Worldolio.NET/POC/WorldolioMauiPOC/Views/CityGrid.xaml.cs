using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.CityGrid;

namespace WorldolioMauiPOC.Views;

public partial class CityGrid : ContentPage
{
    public CityGrid(CityGridViewModel viewModel, ILogger logger, IToolbarHelper toolbarHelper)
    {
        logger.Debug(() => $"CityGrid init");

        BindingContext = viewModel;

        InitializeComponent();

        foreach (var item in toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }
    }
}