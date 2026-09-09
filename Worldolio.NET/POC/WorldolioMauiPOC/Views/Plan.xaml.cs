using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.Plan;

namespace WorldolioMauiPOC.Views;

public partial class Plan : ContentPage
{
    private ILogger _logger;
    private PlanViewModel _viewModel;
    private IToolbarHelper _toolbarHelper;

    public Plan(PlanViewModel viewModel, ILogger logger, IToolbarHelper toolbarHelper)
    {
        logger.Debug(() => $"Plan init");
        InitializeComponent();

        BindingContext = viewModel;

        _logger = logger;
        _viewModel = viewModel;
        _toolbarHelper = toolbarHelper;

        _viewModel.UpdateTimeFromSlider((int)TimeSlider.Value);

        foreach (var item in _toolbarHelper.CreateToolbarItems(true))
        {
            this.ToolbarItems.Add(item);
        }
    }

    // The value we want the slider to increment each time it updates
    private readonly int sliderIncrement = 1;

    // The corrected value for the slider we will be using.
    private int sliderCorrectValue;

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        sliderCorrectValue = (int)(e.NewValue / sliderIncrement) * sliderIncrement;
        _logger.Debug(() => $"Slider_ValueChanged: {e.OldValue} -> {e.NewValue}, {sliderCorrectValue}");
        _viewModel.UpdateTimeFromSlider(sliderCorrectValue);
    }

    private void Left_Clicked(object sender, EventArgs e)
    {
        _logger.Debug(() => $"Left_Clicked");
        if (TimeSlider.Value > TimeSlider.Minimum)
        {
            TimeSlider.Value = TimeSlider.Value - 1;
        }
    }

    private void Right_Clicked(object sender, EventArgs e)
    {
        _logger.Debug(() => $"Left_Clicked");
        if (TimeSlider.Value < TimeSlider.Maximum)
        {
            TimeSlider.Value = TimeSlider.Value + 1;
        }
    }
}