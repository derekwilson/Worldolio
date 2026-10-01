using Worldolio.Data.Logging;
using WorldolioMauiPOC.ViewModels.About;

namespace WorldolioMauiPOC.Utility
{
    public interface IToolbarHelper
    {
        public List<ToolbarItem> CreateToolbarItems(bool showSettings);
        public ToolbarItem CreateBackButton();
    }


    public class ToolbarHelper : IToolbarHelper
    {
        private ILogger _logger;
        private INavigationHelper _navigationHelper;

        public ToolbarHelper(ILogger logger, INavigationHelper navigationHelper)
        {
            _logger = logger;
            _navigationHelper = navigationHelper;
        }

        public ToolbarItem CreateBackButton()
        {
            var item = new ToolbarItem
            {
                Text = "Back",
                IconImageSource = new FontImageSource
                {
                    FontFamily = MaterialSymbolsIconFont.FontName,
                    Glyph = MaterialSymbolsIconFont.IconArrowBack,
                    Size = 20,
                },
                Command = new Command(async () =>
                {
                    await _navigationHelper.ExecuteModalNavigationBackWithDebounceAsync();
                })
            };
            item.IconImageSource.SetAppTheme<Color>(
                    FontImageSource.ColorProperty,
                    Color.FromArgb("#1f1f1f"),      // Light Theme Color - Offblack
                    Colors.White                    // Dark Theme Color
                );
            return item;
        }

        public List<ToolbarItem> CreateToolbarItems(bool showSettings)
        {
            _logger.Debug(() => $"AddToolbarItems settings = {showSettings} ");
            var toolbarItems = new List<ToolbarItem>();
            if (showSettings)
            {
                var settingsImage = new FontImageSource
                {
                    FontFamily = MaterialSymbolsIconFont.FontName,
                    Glyph = MaterialSymbolsIconFont.IconSettings,
                    Size = 20,
                };
                settingsImage.SetAppTheme<Color>(
                    FontImageSource.ColorProperty,
                    Color.FromArgb("#1f1f1f"),      // Light Theme Color - Offblack
                    Colors.White                    // Dark Theme Color
                );
                var settings = new ToolbarItem
                {
                    Text = "Settings",
                    IconImageSource = settingsImage,
                    Command = new Command(async () => await _navigationHelper.ExecuteModalNavigationWithDebounceAsync<Views.Settings>(false))
                };

                toolbarItems.Add(settings);
            }

            var aboutImage = new FontImageSource
            {
                FontFamily = MaterialSymbolsIconFont.FontName,
                Glyph = MaterialSymbolsIconFont.IconInfo,
                Size = 20,
            };
            aboutImage.SetAppTheme<Color>(
                FontImageSource.ColorProperty,
                Color.FromArgb("#1f1f1f"),      // Light Theme Color - Offblack
                Colors.White                    // Dark Theme Color
            );
            var about = new ToolbarItem
            {
                Text = "About",
                IconImageSource = aboutImage,
                Command = new Command(async () =>
                {
                    var page = await _navigationHelper.ExecuteModalNavigationWithDebounceAsync<Views.About>(false);
                    (page?.BindingContext as AboutViewModel)?.Parameter = "Param #2";
                })
            };
            toolbarItems.Add(about);
            _logger.Debug(() => $"AddToolbarItems - complete");
            return toolbarItems;
        }
    }
}
