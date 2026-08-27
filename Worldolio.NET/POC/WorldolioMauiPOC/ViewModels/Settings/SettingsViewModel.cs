using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using Worldolio.Data.Logging;
using WorldolioMauiPOC.Utility;

namespace WorldolioMauiPOC.ViewModels.Settings
{
    public partial class SettingsViewModel : INotifyPropertyChanged
    {
        public ICommand NavigateBack { get; }

        private ILogger _logger;
        private INavigationHelper _navigationHelper;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public SettingsViewModel(ILogger logger, INavigationHelper navigationHelper)
        {
            logger.Debug(() => $"SettingsViewModel init");

            _logger = logger;
            _navigationHelper = navigationHelper;

            NavigateBack = new Command(async () => await _navigationHelper.ExecuteNavigationAsync(".."));
        }

        [RelayCommand]
        private async Task InitAsync()
        {
            _logger.Debug(() => $"SettingsViewModel InitAsync");
        }
    }
}
