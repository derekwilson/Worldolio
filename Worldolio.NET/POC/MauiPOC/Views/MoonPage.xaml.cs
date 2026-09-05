using MauiPOC.Views.Helpers;

namespace MauiPOC.Views;

public partial class MoonPage : ContentPage
{
	public MoonPage()
	{
		InitializeComponent();

        var toolbarHelper = new ToolbarHelper();
        foreach (var item in toolbarHelper.CreateToolbarItems(false))
        {
            this.ToolbarItems.Add(item);
        }
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