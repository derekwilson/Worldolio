using Worldolio.Data.Logging;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.CityGrid;

namespace WorldolioMauiPOC.Views;

public partial class CityGrid : ContentPage
{
    private CityGridViewModel _viewModel;
    private ILogger _logger;
    private IDispatcherTimer _timer;

    public CityGrid(CityGridViewModel viewModel, ILogger logger, IToolbarHelper toolbarHelper)
    {
        _logger = logger;
        _viewModel = viewModel;

        logger.Debug(() => $"CityGrid init");

        BindingContext = viewModel;

        InitializeComponent();

        foreach (var item in toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }

        // Create the timer on the Main Thread's dispatcher
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMinutes(1);
        _timer.Tick += OnTimerTick;
    }

    protected override void OnAppearing()
    {
        _logger.Debug(() => $"CityGrid.OnAppearing");
        base.OnAppearing();

        if (!_timer.IsRunning)
        {
            _timer.Start();
        }
    }

    protected override void OnDisappearing()
    {
        _logger.Debug(() => $"CityGrid.OnDisappearing");
        base.OnDisappearing();

        if (_timer.IsRunning)
        {
            _timer.Stop();
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _logger.Debug(() => $"CityGrid.OnTimerTick");
        _viewModel.UpdateCityGrid();
    }
}