using System.ComponentModel;
using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.ViewModels.Plan
{
    public partial class PlanViewModel : INotifyPropertyChanged
    {
        public int CurrentHour { get; set; } = 0;
        public int CurrentMinute { get; set; } = 0;

        public string CurrentTime
        {
            get
            {
                return $"{CurrentHour}:{CurrentMinute:00}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private ILogger _logger;

        public PlanViewModel(ILogger logger)
        {
            _logger = logger;
        }

        public void UpdateTimeFromSlider(int value)
        {
            // value is in the range 0..96 - every quater of an hour in the day
            if (value < 1 || value > 95)
            {
                CurrentHour = 0;
                CurrentMinute = 0;
            }
            else
            {
                CurrentHour = value / 4;
                CurrentMinute = (value % 4) * 15;
            }
            _logger.Debug(() => $"UpdateTimeFromSlider: {value} -> {CurrentHour}, {CurrentMinute}");
            OnPropertyChanged("CurrentHour");
            OnPropertyChanged("CurrentMinute");
            OnPropertyChanged("CurrentTime");
        }
    }
}
