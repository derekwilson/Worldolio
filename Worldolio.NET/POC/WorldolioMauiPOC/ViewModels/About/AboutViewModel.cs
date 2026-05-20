using System.Windows.Input;
using Worldolio.Data.Logging;
using Worldolio.Data.Repository;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.ViewModels.About
{
    public class AboutViewModel
    {
        public string AppVersion { get; set; }
        public string DotNetVersion { get; set; }
        public string Package { get; set; }
        public string DBVersion { get; set; }
        public string DBPath { get; set; }
        public string LoggingPath { get; set; }

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

            AppVersion = _environmentInformationProvider.GetAppVersion();
            DotNetVersion = Environment.Version.ToString();
            Package = _environmentInformationProvider.GetPackageName();
            var versions = _sraRepository.GetDatabaseSchemaVersions();
            var sra = _sraRepository.GetAllAsync().GetAwaiter().GetResult();
            var dbDate = sra.FirstOrDefault()?.Timestamp.ToString();
            DBVersion = $"Schema: {versions.Item2}, {dbDate}";
            DBPath = _environmentInformationProvider.GetDatabasePath();
            LoggingPath = _environmentInformationProvider.GetLogfileLocation();
        }
    }
}
