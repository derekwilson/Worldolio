using Worldolio.Data.Logging;
using WorldolioMauiPOC.ViewModels.CityGrid;

namespace WorldolioMauiPOC.Views;

public partial class CityGrid : ContentPage
{
    private ILogger _logger;

    public CityGrid(CityGridViewModel viewModel, ILogger logger)
    {
        logger.Debug(() => $"CityGrid init");

        InitializeComponent();

        BindingContext = viewModel;

        _logger = logger;
    }

    private void citiesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _logger.Debug(() => $"CityGrid citiesCollection_SelectionChanged");
    }

    private void DetailsBtn_Clicked(object sender, EventArgs e)
    {
        _logger.Debug(() => $"CityGrid DetailsBtn_Clicked");
    }
}