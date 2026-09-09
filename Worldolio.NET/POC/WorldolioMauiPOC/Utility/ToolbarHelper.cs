using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Utility
{
    public interface IToolbarHelper
    {
        public List<ToolbarItem> CreateToolbarItems(bool showSettings);
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

        public List<ToolbarItem> CreateToolbarItems(bool showSettings)
        {
            _logger.Debug(() => $"AddToolbarItems settings = {showSettings} ");
            var toolbarItems = new List<ToolbarItem>();
            if (showSettings)
            {
                var settingsImage = new FontImageSource
                {
                    FontFamily = "MaterialSymbolsOutlined",
                    Glyph = "\ue8b8",
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
                    Command = new Command(async () => await _navigationHelper.ExecuteModalNavigationAsync<Views.Settings>())
                };

                toolbarItems.Add(settings);
            }

            var aboutImage = new FontImageSource
            {
                FontFamily = "MaterialSymbolsOutlined",
                Glyph = "\ue88e",
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
                Command = new Command(async () => await _navigationHelper.ExecuteModalNavigationAsync<Views.About>())
            };
            toolbarItems.Add(about);
            _logger.Debug(() => $"AddToolbarItems - complete");
            return toolbarItems;
        }
    }
}
