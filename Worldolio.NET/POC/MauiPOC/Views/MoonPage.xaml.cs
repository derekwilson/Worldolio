namespace MauiPOC.Views;

public partial class MoonPage : ContentPage
{
	public MoonPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID
        // we want the toolbar to have text
        Title = "Worldolio";
#endif

#if WINDOWS
        this.Window.Title = "Worldolio - Moon";
        Title = "";
#endif
    }
}