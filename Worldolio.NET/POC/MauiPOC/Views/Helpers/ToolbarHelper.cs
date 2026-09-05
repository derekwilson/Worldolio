namespace MauiPOC.Views.Helpers
{
    internal class ToolbarHelper
    {
        public List<ToolbarItem> CreateToolbarItems(bool showSettings)
        {
            //_logger.Debug(() => $"AddToolbarItems settings = {showSettings} ");
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
                    //Command = new Command(async () => await _navigationHelper.ExecuteModalNavigationAsync<Views.Settings>())
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
                //Command = new Command(async () => await _navigationHelper.ExecuteModalNavigationAsync<Views.About>())
                Command = new Command(async () =>
                {
                    var page = new AboutPage();
                    var navigator = App.Current?.MainPage?.Navigation;
                    await navigator?.PushModalAsync(new NavigationPage(page), true);
                })
            };
            toolbarItems.Add(about);
            //_logger.Debug(() => $"AddToolbarItems - complete");
            return toolbarItems;
        }
    }
}
