using System.ComponentModel;
using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.ViewModels.Map
{
    public partial class MapViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private ILogger _logger;

        public MapViewModel(ILogger logger)
        {
            logger.Debug(() => $"MapViewModel init");
            _logger = logger;
        }
    }
}
