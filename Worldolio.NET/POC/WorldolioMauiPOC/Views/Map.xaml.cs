using Worldolio.Data.Logging;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.Map;
using WorldolioMauiPOC.Views.Drawable;

namespace WorldolioMauiPOC.Views;

public partial class Map : ContentPage
{
    private ILogger _logger;
    private MapDrawable _mapDrawable;
    private ISystemTimeProvider _timeProvider;
    private IDispatcherTimer _timer;

    public Map(
        MapViewModel viewModel,
        ILogger logger,
        IToolbarHelper toolbarHelper,
        MapDrawable drawable,
        ISystemTimeProvider timeProvider)
    {
        logger.Debug(() => $"Map init");
        _logger = logger;
        _mapDrawable = drawable;
        _timeProvider = timeProvider;

        BindingContext = viewModel;

        InitializeComponent();

        // if we want the drawable to be DI then this is the way we do it
        MapGraphicsView.Drawable = drawable;

        foreach (var item in toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }

        // Create the timer on the Main Thread's dispatcher
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMinutes(15);
        _timer.Tick += OnTimerTick;
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

    private void UpdateMap()
    {
        // there isnt a lot of point making a bindable property if we are not going to instatiate it in XAML
        // so we will just make it a property
        _mapDrawable.UtcTime = _timeProvider.GetUtcNow();
        MapGraphicsView.Invalidate();
    }

    protected override void OnAppearing()
    {
        _logger.Debug(() => $"Map.OnAppearing");
        base.OnAppearing();

        UpdateMap();

        if (!_timer.IsRunning)
        {
            _timer.Start();
        }
    }

    protected override void OnDisappearing()
    {
        _logger.Debug(() => $"Map.OnAppearing");
        base.OnDisappearing();

        if (_timer.IsRunning)
        {
            _timer.Stop();
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _logger.Debug(() => $"Map.OnTimerTick");
        UpdateMap();
    }
}