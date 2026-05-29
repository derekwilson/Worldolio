using Worldolio.Data.Logging;
using WorldolioMauiPOC.ViewModels.Plan;

namespace WorldolioMauiPOC.Views;

public partial class Plan : ContentPage
{
    private ILogger _logger;
    private PlanViewModel _viewModel;

    public Plan(PlanViewModel viewModel, ILogger logger)
    {
        logger.Debug(() => $"Plan init");
        InitializeComponent();

        BindingContext = viewModel;

        _logger = logger;
        _viewModel = viewModel;
    }

    // The value we want the slider to increment each time it updates
    private readonly int sliderIncrement = 1;

    // The corrected value for the slider we will be using.
    private int sliderCorrectValue;

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        _logger.Debug(() => $"Slider_ValueChanged: {e.OldValue} -> {e.NewValue}");
        sliderCorrectValue = (int)(e.NewValue / sliderIncrement) * sliderIncrement;
        _viewModel.UpdateTimeFromSlider(sliderCorrectValue);
    }
}