using Worldolio.Data.Logging;
using Worldolio.Data.Utility;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.Map;
using WorldolioMauiPOC.Views.Drawable;

namespace WorldolioMauiPOC.Views;

public partial class Map : ContentPage
{
    private const int FORCE_REFRESH_MINUTES = 15;

    private ILogger _logger;
    private MapDrawable _mapDrawable;
    private ISystemTimeProvider _timeProvider;
    private IDialogHelper _dialogHelper;

    private IDispatcherTimer _timer;
    private DateTime _lastForcedRefreshTime = DateTime.MinValue;

    public Map(
        MapViewModel viewModel,
        ILogger logger,
        IToolbarHelper toolbarHelper,
        MapDrawable drawable,
        ISystemTimeProvider timeProvider,
        IDialogHelper dialogHelper)
    {
        logger.Debug(() => $"Map init");
        _logger = logger;
        _mapDrawable = drawable;
        _timeProvider = timeProvider;
        _dialogHelper = dialogHelper;

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
        _timer.Interval = TimeSpan.FromMinutes(FORCE_REFRESH_MINUTES);
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
        // there isnt a lot of point making a bindable property if we are not going to instatiate the control in XAML
        // so we will just make it a property
        _mapDrawable.UtcTime = _timeProvider.GetUtcNow();
        MapGraphicsView.Invalidate();
    }

    private bool NeedUpdateBecauseTimeElapsed()
    {
        var elapsed = _lastForcedRefreshTime - _timeProvider.GetUtcNow();
        return elapsed.TotalMinutes > FORCE_REFRESH_MINUTES;
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _logger.Debug(() => $"Map.OnTimerTick");
        UpdateMap();
        _lastForcedRefreshTime = _timeProvider.GetUtcNow();
    }

    protected override void OnAppearing()
    {
        _logger.Debug(() => $"Map.OnAppearing");
        base.OnAppearing();

        if (_mapDrawable.NeedUpdateBecauseDataChanged || NeedUpdateBecauseTimeElapsed())
        {
            _logger.Debug(() => $"Map.OnAppearing - refresh map");
            UpdateMap();
            _lastForcedRefreshTime = _timeProvider.GetUtcNow();
        }

        if (!_timer.IsRunning)
        {
            _timer.Start();
        }
    }

    protected override void OnDisappearing()
    {
        _logger.Debug(() => $"Map.OnDisappearing");
        base.OnDisappearing();

        if (_timer.IsRunning)
        {
            _timer.Stop();
        }
    }

    private void MapGraphicsView_MoveHoverInteraction(object sender, TouchEventArgs e)
    {
        // Capture the first pointer contact coordinate position
        PointF hoverPosition = e.Touches[0];
        //_logger.Debug(() => $"Map.MapGraphicsView_MoveHoverInteraction {hoverPosition.X} {hoverPosition.Y}");
        MapTooltipLabel.Text = _mapDrawable.GetTooltipText(hoverPosition);
    }

    private void MapGraphicsView_EndHoverInteraction(object sender, EventArgs e)
    {
        _logger.Debug(() => $"Map.MapGraphicsView_EndHoverInteraction");
        MapTooltipLabel.Text = "  ";
    }

    private async void MapGraphicsView_EndInteraction(object sender, TouchEventArgs e)
    {
        // Capture the first pointer contact coordinate position
        PointF clickPosition = e.Touches[0];

        _logger.Debug(() => $"Map.MapGraphicsView_EndInteraction {clickPosition.X},{clickPosition.Y}");
        //await _dialogHelper.ShowAlertAsync("Alert", $"Pos = {clickPosition.X},{clickPosition.Y}");
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var pos = e.GetPosition((View)sender);
        if (pos is not null)
        {
            Point clickPosition = (Point) pos;
            _logger.Debug(() => $"Map.TapGestureRecognizer_Tapped {clickPosition.X},{clickPosition.Y}");
            await _dialogHelper.ShowAlertAsync("Alert", $"Pos = {clickPosition.X},{clickPosition.Y}");
        }
    }
}