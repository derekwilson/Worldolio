using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using Worldolio.Data.Logging;
using Worldolio.Data.Repository;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.ViewModels.About
{
    public partial class AboutViewModel : INotifyPropertyChanged
    {
        public bool HasError { get; set; } = false;
        public bool Loading { get; set; } = false;

        public string AppVersion { get; set; } = "";
        public string DotNetVersion { get; set; } = "";
        public string Package { get; set; } = "";
        public string DBVersion { get; set; } = "";
        public string DBPath { get; set; } = "";
        public string LoggingPath { get; set; } = "";
        public string TzDbVersion { get; set; } = "";

        public ICommand NavigateBack { get; }

        private ILogger _logger;
        private ISchemaRevisionAuditRepository _sraRepository;
        private IEnvironmentInformationProvider _environmentInformationProvider;
        private INavigationHelper _navigationHelper;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public AboutViewModel(ILogger logger, IEnvironmentInformationProvider environmentInformationProvider, ISchemaRevisionAuditRepository sraRepository, INavigationHelper navigationHelper)
        {
            logger.Debug(() => $"AboutViewModel init");

            _logger = logger;
            _environmentInformationProvider = environmentInformationProvider;
            _sraRepository = sraRepository;
            _navigationHelper = navigationHelper;

            NavigateBack = new Command(async () => await _navigationHelper.ExecuteNavigationAsync(".."));
        }

        [RelayCommand]
        private async Task InitAsync()
        {
            _logger.Debug(() => $"AboutViewModel InitAsync");

            try
            {
                AppVersion = _environmentInformationProvider.GetAppVersion();
                OnPropertyChanged(nameof(AppVersion));

                DotNetVersion = Environment.Version.ToString();
                OnPropertyChanged(nameof(DotNetVersion));

                Package = _environmentInformationProvider.GetPackageName();
                OnPropertyChanged(nameof(Package));

                var versions = _sraRepository.GetDatabaseSchemaVersions();
                var sra = await _sraRepository.GetAllAsync();
                var dbDate = sra.FirstOrDefault()?.Timestamp.ToString();
                DBVersion = $"Schema: {versions.Item2}, {dbDate}";
                OnPropertyChanged(nameof(DBVersion));

                DBPath = _environmentInformationProvider.GetDatabasePath();
                OnPropertyChanged(nameof(DBPath));

                LoggingPath = _environmentInformationProvider.GetLogfileLocation();
                OnPropertyChanged(nameof(LoggingPath));

                TzDbVersion = _environmentInformationProvider.GetIanaTzDatabaseVersion();
                OnPropertyChanged(nameof(TzDbVersion));
            }
            catch
            {
                HasError = true;
                OnPropertyChanged(nameof(HasError));
            }
            finally
            {
                Loading = false;
                OnPropertyChanged(nameof(Loading));
            }
        }
    }
}
