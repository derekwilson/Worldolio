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
            MainPage = new AppShell(_logger);
        }
    }
}
