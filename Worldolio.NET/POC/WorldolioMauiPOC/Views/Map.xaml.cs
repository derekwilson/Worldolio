using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.Map;
using WorldolioMauiPOC.Views.Drawable;

namespace WorldolioMauiPOC.Views;

public partial class Map : ContentPage
{
    private ILogger _logger;
    
    public Map(
        MapViewModel viewModel, 
        ILogger logger, 
        IToolbarHelper toolbarHelper,
        MapDrawable drawable
        )
	{
        logger.Debug(() => $"Map init");
        _logger = logger;

        BindingContext = viewModel;
        
        InitializeComponent();

        // Assign the DI-injected drawable to your XAML GraphicsView
        MapGraphicsView.Drawable = drawable;

        foreach (var item in toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }
    }

    private void Grid_SizeChanged(object sender, EventArgs e)
    {
        if (sender is Grid containerGrid)
        {
            // Calculate target height for a 16:9 aspect ratio
            //double targetHeight = containerGrid.Width * (9.0 / 16.0);
            // Calculate target height for a 1:2 aspect ratio - this needs to match our resource image
            double targetHeight = containerGrid.Width * (1.0 / 2.0);

            _logger.Debug(() => $"Grid_SizeChanged {containerGrid.Width} == {targetHeight}");

            // Enforce the height on the GraphicsView
            MapGraphicsView.HeightRequest = targetHeight;
        }
    }
}