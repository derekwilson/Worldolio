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
            _logger.Debug(() => $"App Create App Shell");
            return new Window(new AppShell(_logger));
        }
#endif

#if ANDROID
        protected override Window CreateWindow(IActivationState? activationState)
        {
            _logger.Debug(() => $"App Create Tabbed Page");
            return new Window(new AppTabbedPage(_logger, MauiProgram.Services));
        }
#endif
    }
}
