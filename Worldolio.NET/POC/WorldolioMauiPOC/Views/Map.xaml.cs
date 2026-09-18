using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.Map;

namespace WorldolioMauiPOC.Views;

public partial class Map : ContentPage
{
	public Map(MapViewModel viewModel, ILogger logger, IToolbarHelper toolbarHelper)
	{
        logger.Debug(() => $"Map init");

        BindingContext = viewModel;
        
        InitializeComponent();

        foreach (var item in toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }
    }
}