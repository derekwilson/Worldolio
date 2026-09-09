namespace MauiPOC;

public partial class AppTabbedPage : TabbedPage
{
	public AppTabbedPage()
	{
		InitializeComponent();

#if WINDOWS
        _previousPage = CurrentPage;
        CurrentPageChanged += OnCurrentPageChanged;

        System.Diagnostics.Debug.WriteLine("AppTabbedPage - start");
#endif
    }

    #region debounce tab switching

    private Page? _previousPage = null;
    private bool _isReverting = false;
    private DateTime _lastNavigationTime = DateTime.MinValue;
    private const int DebounceDelayMilliseconds = 300;

    private async void OnCurrentPageChanged(object? sender, EventArgs e)
    {
        await Task.Delay(2);

        if (_isReverting) return;

        bool isValid = CheckMyCondition(); // Your validation logic

        if (!isValid)
        {
            System.Diagnostics.Debug.WriteLine("OnCurrentPageChanged ** too fast");
            _isReverting = true;
            CurrentPage = _previousPage; // Revert back
            _isReverting = false;

        }
        else
        {
            System.Diagnostics.Debug.WriteLine("OnCurrentPageChanged - switch");
            _previousPage = CurrentPage; // Update last valid page
        }
    }

    private bool CheckMyCondition()
    {
        var now = DateTime.UtcNow;
        if ((now - _lastNavigationTime).TotalMilliseconds < DebounceDelayMilliseconds)
        {
            // Cancel the navigation if it's too fast
            return false;
        }

        _lastNavigationTime = now;
        return true;
    }

    #endregion
}