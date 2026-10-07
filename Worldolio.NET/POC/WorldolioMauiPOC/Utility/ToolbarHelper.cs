using System.Windows.Input;
using Worldolio.Data.Logging;
using WorldolioMauiPOC.ViewModels.About;

namespace WorldolioMauiPOC.Utility
{
    public interface IToolbarHelper
    {
        public List<ToolbarItem> CreateToolbarItems(bool showSettings);
        public ToolbarItem CreateBackButton();
        public ToolbarItem CreateDoneButton();
    }


    public class ToolbarHelper : IToolbarHelper
    {
        private ILogger _logger;
        private INavigationHelper _navigationHelper;
        private IResourceProvider _resourceProvider;

        public ToolbarHelper(ILogger logger, INavigationHelper navigationHelper, IResourceProvider resourceProvider)
        {
            _logger = logger;
            _navigationHelper = navigationHelper;
            _resourceProvider = resourceProvider;
        }

        private ToolbarItem CreateToolbarItem(
            string text,
            string iconFont, 
            string iconGlyph, 
            Color lightThemeColour,
            Color darkThemeColour,
            ICommand command
            )
        {
            var item = new ToolbarItem
            {
                Text = text,
                IconImageSource = new FontImageSource
                {
                    FontFamily = iconFont,
                    Glyph = iconGlyph,
                    Size = 20,
                },
                Command = command
            };
            item.IconImageSource.SetAppTheme<Color>(FontImageSource.ColorProperty,lightThemeColour,darkThemeColour);
            return item;
        }

        public ToolbarItem CreateBackButton()
        {
            return CreateToolbarItem(
                "Back",
                MaterialSymbolsIconFont.FontName,
                MaterialSymbolsIconFont.IconArrowBack,
                _resourceProvider.GetResource<Color>("Offblack", Colors.Black),     // Light Theme Color
                _resourceProvider.GetResource<Color>("White", Colors.White),        // Dark Theme Color
                new Command(async () =>
                {
                    await _navigationHelper.ExecuteModalNavigationBackWithDebounceAsync();
                })
                );
        }

        public ToolbarItem CreateDoneButton()
        {
            return CreateToolbarItem(
                "Done",
                MaterialSymbolsIconFont.FontName,
                MaterialSymbolsIconFont.IconClose,
                _resourceProvider.GetResource<Color>("Offblack", Colors.Black),     // Light Theme Color
                _resourceProvider.GetResource<Color>("White", Colors.White),        // Dark Theme Color
                new Command(async () =>
                {
                    await _navigationHelper.ExecuteModalNavigationBackWithDebounceAsync();
                })
                );
        }

        public List<ToolbarItem> CreateToolbarItems(bool showSettings)
        {
            _logger.Debug(() => $"AddToolbarItems settings = {showSettings} ");
            var toolbarItems = new List<ToolbarItem>();
            if (showSettings)
            {
                var settingsItem = CreateToolbarItem(
                    "Settings",
                    MaterialSymbolsIconFont.FontName,
                    MaterialSymbolsIconFont.IconSettings,
                    _resourceProvider.GetResource<Color>("Offblack", Colors.Black),     // Light Theme Color
                    _resourceProvider.GetResource<Color>("White", Colors.White),        // Dark Theme Color
                    new Command(async () =>
                    {
                        await _navigationHelper.ExecuteModalNavigationWithDebounceAsync<Views.Settings>(false);
                    })
                );
                toolbarItems.Add(settingsItem);
            }

            var aboutItem = CreateToolbarItem(
                "About",
                MaterialSymbolsIconFont.FontName,
                MaterialSymbolsIconFont.IconInfo,
                _resourceProvider.GetResource<Color>("Offblack", Colors.Black),     // Light Theme Color
                _resourceProvider.GetResource<Color>("White", Colors.White),        // Dark Theme Color
                new Command(async () =>
                {
                    await _navigationHelper.ExecuteModalNavigationWithDebounceAsync<Views.About>(false);
                })
            );
            toolbarItems.Add(aboutItem);

            _logger.Debug(() => $"AddToolbarItems - complete");
            return toolbarItems;
        }
    }
}
