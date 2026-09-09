using Worldolio.Data.Logging;

namespace WorldolioMauiPOC
{
    public partial class App : Application
    {
        private ILogger _logger = null!;

        public App(ILogger logger)
        {
            InitializeComponent();

            _logger = logger;
            _logger.Debug(() => $"App Started");
        }

#if WINDOWS
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // WINDOWS
            // TabbedPage really doesnt work well in Windows
            // fast switching tabs causes WinUi to crash, see "Tab crah.txt" in Notes
            // this *appears* to be fixed by a combination of .Net 10 and debouncing
            //
            // both solutions package the same ContenPage collections, CityGrid, Plan, Moon

            _logger.Debug(() => $"App Create App Shell");
            return new Window(new AppShell(_logger));
        }
#endif

#if ANDROID
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // ANDROID
            // TabbedPage is much better than the shell for Android
            // things that AppShell does not do on Android
            //  - tabs at the top of the screen
            //  - swipe left/right to switch tabs
            //
            // both solutions package the same ContenPage collections, CityGrid, Plan, Moon

            _logger.Debug(() => $"App Create Tabbed Page");
            return new Window(new AppTabbedPage(_logger, MauiProgram.Services));
        }
#endif
    }
}
