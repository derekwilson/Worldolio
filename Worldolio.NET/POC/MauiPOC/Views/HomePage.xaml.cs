using System.Windows.Input;

namespace MauiPOC.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
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
        this.Window.Title = "Worldolio - Home";
        Title = "";
#endif
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        var page = new AboutPage();
        await Navigation.PushModalAsync(new NavigationPage(page), true);
    }
}