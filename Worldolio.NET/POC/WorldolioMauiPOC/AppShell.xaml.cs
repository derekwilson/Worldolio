using Worldolio.Data.Logging;

namespace WorldolioMauiPOC
{
    public partial class AppShell : Shell
    {
        private ILogger _logger = null!;

        public AppShell(ILogger logger)
        {
            InitializeComponent();

#if DEBUG
            this.Title = this.Title + " (Debug)";
#endif
            _logger = logger;
            _logger.Debug(() => $"App Shell Started");
        }

#if WINDOWS

        #region debounce tab switch

        private DateTime _lastNavigationTime = DateTime.MinValue;
        private const int DebounceDelayMilliseconds = 300;

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            // Only debounce if the navigation is triggered by tab switching (ShellSection)
            if (args.Source == ShellNavigationSource.ShellSectionChanged)
            {
                _logger.Debug(() => $"OnNavigating check tab switch");
                var now = DateTime.UtcNow;
                if ((now - _lastNavigationTime).TotalMilliseconds < DebounceDelayMilliseconds)
                {
                    // Cancel the navigation if it's too fast
                    args.Cancel();
                    _logger.Debug(() => $"OnNavigating * debounced as too fast *");
                    return;
                }

                _lastNavigationTime = now;
            }
        }

        protected async override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            await Task.Delay(1);
        }

    #endregion

#endif

    }
}
