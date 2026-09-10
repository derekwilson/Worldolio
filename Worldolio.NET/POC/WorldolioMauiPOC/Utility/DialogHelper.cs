using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Utility
{
    public interface IDialogHelper
    {
        Task ShowAlertAsync(string title, string message);
    }

    public class DialogHelper : IDialogHelper
    {
        private ILogger _logger;

        public DialogHelper(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ShowAlertAsync(string title, string message)
        {
            var window = App.Current?.Windows[0].Page;
            if (window == null)
            {
                _logger.Warning(() => $"DialogHelper ShowAlertAsync - NULL window");
            }
            else
            {
                await window.DisplayAlert(title, message, "OK");
            }
        }
    }
}
