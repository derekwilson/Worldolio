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

    private NavigationPage BuildOneTab<PAGE>(string title, string iconFontFamily, string glyph)
        where PAGE : ContentPage
    {
        return new NavigationPage(_serviceProvider.GetRequiredService<PAGE>())
        {
            Title = title,
            IconImageSource = new FontImageSource
            {
                FontFamily = iconFontFamily,
                Glyph = glyph,
            }
        };
    }

    private void InitTabs()
    {
        _logger.Debug(() => $"AppTabbedPage InitTabs");

        this.Children.Add(BuildOneTab<CityGrid>("Home", MaterialSymbolsIconFont.FontName, MaterialSymbolsIconFont.IconGlobe));
        this.Children.Add(BuildOneTab<Plan>("Plan", MaterialSymbolsIconFont.FontName, MaterialSymbolsIconFont.IconCalendarMonth));
        this.Children.Add(BuildOneTab<Moon>("Moon", MaterialSymbolsIconFont.FontName, MaterialSymbolsIconFont.IconBedtime));

        _logger.Debug(() => $"AppTabbedPage InitTabs - done");
    }
}