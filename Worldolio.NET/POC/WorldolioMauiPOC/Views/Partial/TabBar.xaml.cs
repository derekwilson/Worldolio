using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.Views.Partial;

public partial class TabBar : ContentView
{
    public List<ToolbarItem> ToolbarItems { get; set; } = null!;

    public bool ShowSettings
    {
        get
        {
            _logger.Debug(() => $"ShowSettings get");
            return (bool)GetValue(ShowSettingsProperty);
        }
        set
        {
            _logger.Debug(() => $"ShowSettings set = {value} ");
            SetValue(ShowSettingsProperty, value);
            AddToolbarItems(value);
        }
    }

    public static readonly BindableProperty
        ShowSettingsProperty = BindableProperty.Create(
            nameof(ShowSettings),
            typeof(bool), 
            typeof(TabBar), 
            false,
            BindingMode.TwoWay
            );

    private ILogger _logger;
    private INavigationHelper _navigationHelper;

    public TabBar()
	{
		InitializeComponent();
        _logger = MauiProgram.Services.GetRequiredService<ILogger>();
        _navigationHelper = MauiProgram.Services.GetRequiredService<INavigationHelper>();
        _logger.Debug(() => $"TabBar init");
        AddToolbarItems(ShowSettings);
    }

    private void AddToolbarItems(bool showSettings)
    {
        _logger.Debug(() => $"AddToolbarItems settings = {showSettings} ");
        ToolbarItems = new List<ToolbarItem>();
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
                Colors.Black,  // Light Theme Color
                Colors.White   // Dark Theme Color
            );
            var settings = new ToolbarItem
            {
                Text = "Settings",
                IconImageSource = settingsImage,
                Command = new Command(async () => await _navigationHelper.ExecuteModalNavigationAsync<Views.Settings>())
            };

            ToolbarItems.Add(settings);
        }

        var aboutImage = new FontImageSource
        {
            FontFamily = "MaterialSymbolsOutlined",
            Glyph = "\ue88e",
            Size = 20,
        };
        aboutImage.SetAppTheme<Color>(
            FontImageSource.ColorProperty,
            Colors.Black,  // Light Theme Color
            Colors.White   // Dark Theme Color
        );
        var about = new ToolbarItem
        {
            Text = "About",
            IconImageSource = aboutImage,
            Command = new Command(async () => await _navigationHelper.ExecuteModalNavigationAsync<Views.About>())
        };

        ToolbarItems.Add(about);
        _logger.Debug(() => $"AddToolbarItems - complete");
    }

    private void OnSettingsClicked(object obj)
    {
        _logger.Debug(() => $"OnSettingsClicked");
    }
}