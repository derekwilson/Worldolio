using System.Collections.ObjectModel;
using System.ComponentModel;
using Worldolio.Data.Logging;
using Worldolio.Data.Model;

namespace WorldolioMauiPOC.ViewModels.AddCity
{
    public partial class AddCityViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<City> Cities { get; set; } = new ObservableCollection<City>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private ILogger _logger;

        public AddCityViewModel(ILogger logger)
        {
            logger.Debug(() => $"AddCityViewModel init");

            _logger = logger;
        }

        internal void SetCities(ICollection<City> cities)
        {
            _logger.Debug(() => $"AddCityViewModel SetCities");
            Cities = new ObservableCollection<City>(cities);
            OnPropertyChanged(nameof(Cities));
        }
    }
}
