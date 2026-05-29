using System.ComponentModel;

namespace WorldolioMauiPOC.ViewModels.Plan
{
    public partial class PlanViewModel : INotifyPropertyChanged
    {
        public int CurrentHour { get; set; } = -1;
        public int CurrentMinute { get; set; } = -1;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void UpdateTimeFromSlider(int value)
        {
            // value is in the range 0..96 - every quater of an hour in the day
            if (value < 1 || value > 95)
            {
                CurrentHour = 0;
                CurrentHour = 0;
            }
            else
            {
                CurrentHour = value / 4;
                CurrentMinute = (value % 4) * 15;
            }
            OnPropertyChanged("CurrentHour");
            OnPropertyChanged("CurrentMinute");
        }
    }
}
