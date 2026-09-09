using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.CityGrid;

namespace WorldolioMauiPOC.Views;

public partial class CityGrid : ContentPage
{
    private ILogger _logger;
    private IToolbarHelper _toolbarHelper;

    public CityGrid(CityGridViewModel viewModel, ILogger logger, IToolbarHelper toolbarHelper)
    {
        logger.Debug(() => $"CityGrid init");

        InitializeComponent();

        BindingContext = viewModel;

        _logger = logger;
        _toolbarHelper = toolbarHelper;

        foreach (var item in _toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _logger.Debug(() => $"CityGrid OnAppearing");
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