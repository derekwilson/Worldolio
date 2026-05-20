using System.Windows.Input;
using Worldolio.Data.Logging;
using Worldolio.Data.Repository;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.ViewModels.About
{
    public class AboutViewModel
    {
        public ICommand NavigateBack { get; }

        private ILogger _logger;
        private ISchemaRevisionAuditRepository _sraRepository;
        private IEnvironmentInformationProvider _environmentInformationProvider;
        private INavigationHelper _navigationHelper;

        public AboutViewModel(ILogger logger, IEnvironmentInformationProvider environmentInformationProvider, ISchemaRevisionAuditRepository sraRepository, INavigationHelper navigationHelper)
        {
            logger.Debug(() => $"AboutViewModel init");

            _logger = logger;
            _environmentInformationProvider = environmentInformationProvider;
            _sraRepository = sraRepository;
            _navigationHelper = navigationHelper;

            NavigateBack = new Command(async () => await _navigationHelper.ExecuteNavigationAsync(".."));
        }
    }
}
