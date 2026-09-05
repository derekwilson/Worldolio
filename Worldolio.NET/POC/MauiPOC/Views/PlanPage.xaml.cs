using MauiPOC.Views.Helpers;

namespace MauiPOC.Views;

public partial class PlanPage : ContentPage
{
	public PlanPage()
	{
		InitializeComponent();

        var toolbarHelper = new ToolbarHelper();
        foreach (var item in toolbarHelper.CreateToolbarItems(true))
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
        this.Window.Title = "Worldolio - Time Planner";
        Title = "";
#endif
    }
}