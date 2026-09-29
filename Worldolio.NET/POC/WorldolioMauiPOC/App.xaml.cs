using Worldolio.Data.Logging;
using WorldolioMauiPOC.AppSettings;

namespace WorldolioMauiPOC
{
    public partial class App : Application
    {
        private ILogger _logger = null!;
        private IUserSettings _userSettings = null!;

        public App(ILogger logger, IUserSettings userSettings)
        {
            InitializeComponent();

            _logger = logger;
            _userSettings = userSettings;

            _logger.Debug(() => $"App Started");
        }

#if WINDOWS
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // WINDOWS
            // TabbedPage really doesnt work well in Windows
            // fast switching tabs causes WinUi to crash, see "Tab crah.txt" in Notes
            // this *appears* to be fixed in AppShell by a combination of .Net 10 and debouncing
            //
            // both solutions package the same ContenPage collections, CityGrid, Plan, Moon, Map etc

            _logger.Debug(() => $"App Create App Shell");
            //var newWindow = new Window(new AppShell(_logger));

            var newWindow = new Window(new AppShell(_logger))
            {
                Height = _userSettings.WindowHeight,
                Width = _userSettings.WindowWidth
            };

            newWindow.Created += (sender, args) =>
            {
                _logger.Debug(() => $"App window created");
                if (sender is Window mauiWindow)
                {
                    if (mauiWindow.Handler?.PlatformView is Microsoft.Maui.MauiWinUIWindow nativeWindow)
                    {
                        // there is no cross platform mechanism for getting the window state - minimised, maximised, restored etc 
                        _logger.Debug(() => $"attaching native window handlers");
                        nativeWindow.AppWindow.Closing += (sender, args) =>
                        {
                            if (sender.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                            {
                                // Check current state via presenter status
                                if (presenter.State == Microsoft.UI.Windowing.OverlappedPresenterState.Restored)
                                {
                                    // we only want to save the settings if the window is displayed
                                    _logger.Debug(() => $"App window - Closing - Restored: {newWindow.X}, {newWindow.Y}, {newWindow.Width}, {newWindow.Height}");
                                    _userSettings.WindowWidth = newWindow.Width;
                                    _userSettings.WindowHeight = newWindow.Height;
                                }
                            }
                        };
                    }
                }
            };

            _logger.Debug(() => $"App.CreateWindow: complete");
            return newWindow;
        }
#endif

#if ANDROID
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // ANDROID
            // TabbedPage is much better than the shell for Android
            // things that AppShell does not do on Android
            //  - tabs at the top of the screen (this maybe possible with top tabs but they dont support icons)
            //  - swipe left/right to switch tabs
            //
            // both solutions package the same ContenPage collections, CityGrid, Plan, Moon, Map etc

            _logger.Debug(() => $"App Create Tabbed Page");
            return new Window(new AppTabbedPage(_logger, MauiProgram.Services));
        }
#endif
    }
}
