using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
using WorldolioMauiPOC.ViewModels.AddCity;

namespace WorldolioMauiPOC.Views;

public partial class AddCity : ContentPage
{
    private ILogger _logger;

    public AddCity(
        AddCityViewModel viewModel,
        IToolbarHelper toolbarHelper,
        ILogger logger
        )
    {
        logger.Debug(() => $"AddCity init");
        BindingContext = viewModel;

        InitializeComponent();

        this.ToolbarItems.Add(toolbarHelper.CreateDoneButton());

        _logger = logger;
    }
}