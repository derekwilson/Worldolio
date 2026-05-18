using Worldolio.Data.Logging;
using WorldolioMauiPOC.ViewModels.CityGrid;

namespace WorldolioMauiPOC.Views;

public partial class CityGrid : ContentPage
{
    private ILogger _logger;

    public CityGrid(CityGridViewModel viewModel, ILogger logger)
    {
        InitializeComponent();

        BindingContext = viewModel;
        _logger = logger;

        _logger.Debug(() => $"CityGrid init");
    }

    private void citiesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _logger.Debug(() => $"CityGrid citiesCollection_SelectionChanged");
    }

    private void Toolbar_Settings(object sender, EventArgs e)
    {
        _logger.Debug(() => $"CityGrid Toolbar_Settings");
    }

    private void Toolbar_About(object sender, EventArgs e)
    {
        _logger.Debug(() => $"CityGrid Toolbar_About");
    }
}