using Worldolio.Data.Logging;
using WorldolioMauiPOC.Views;

namespace WorldolioMauiPOC;

public partial class AppTabbedPage : TabbedPage
{
    private ILogger _logger = null!;
    private readonly IServiceProvider _serviceProvider;

    public AppTabbedPage(ILogger logger, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _logger = logger;
        _serviceProvider = serviceProvider;
        _logger.Debug(() => $"AppTabbedPage Started");

        InitTabs();
    }

    private void InitTabs()
    {
        _logger.Debug(() => $"AppTabbedPage InitTabs");
        var homeImage = new FontImageSource
        {
            FontFamily = "MaterialSymbolsOutlined",
            Glyph = "\ue64c",
        };

        var homePage = _serviceProvider.GetRequiredService<CityGrid>();
        var tab1 = new NavigationPage(homePage);
        tab1.Title = "Home";
        tab1.IconImageSource = homeImage;

        var planImage = new FontImageSource
        {
            FontFamily = "MaterialSymbolsOutlined",
            Glyph = "\uebcc",
        };

        var planPage = _serviceProvider.GetRequiredService<Plan>();
        var tab2 = new NavigationPage(planPage);
        tab2.Title = "Plan";
        tab2.IconImageSource = planImage;

        var moonImage = new FontImageSource
        {
            FontFamily = "MaterialSymbolsOutlined",
            Glyph = "\uef44",
        };

        var moonPage = _serviceProvider.GetRequiredService<Moon>();
        var tab3 = new NavigationPage(moonPage);
        tab3.Title = "Moon";
        tab3.IconImageSource = moonImage;

        this.Children.Add(tab1);
        this.Children.Add(tab2);
        this.Children.Add(tab3);
        _logger.Debug(() => $"AppTabbedPage InitTabs - done");
    }
}