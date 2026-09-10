using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;
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

        var homeTab = new NavigationPage(_serviceProvider.GetRequiredService<CityGrid>())
        {
            Title = "Home",
            IconImageSource = new FontImageSource
            {
                FontFamily = MaterialSymbolsIconFont.FontName,
                Glyph = MaterialSymbolsIconFont.IconGlobe,
            }
        };

        var planTab = new NavigationPage(_serviceProvider.GetRequiredService<Plan>())
        {
            Title = "Plan",
            IconImageSource = new FontImageSource
            {
                FontFamily = MaterialSymbolsIconFont.FontName,
                Glyph = MaterialSymbolsIconFont.IconCalendarMonth,
            }
        };

        var moonTab = new NavigationPage(_serviceProvider.GetRequiredService<Moon>())
        {
            Title = "Moon",
            IconImageSource = new FontImageSource
            {
                FontFamily = MaterialSymbolsIconFont.FontName,
                Glyph = MaterialSymbolsIconFont.IconBedtime,
            }
        };

        this.Children.Add(homeTab);
        this.Children.Add(planTab);
        this.Children.Add(moonTab);
        _logger.Debug(() => $"AppTabbedPage InitTabs - done");
    }
}