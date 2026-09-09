using System.ComponentModel;
using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.ViewModels.Moon
{
    public partial class MoonViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private ILogger _logger;

        public MoonViewModel(ILogger logger)
        {
            _logger = logger;
        }
    }
}