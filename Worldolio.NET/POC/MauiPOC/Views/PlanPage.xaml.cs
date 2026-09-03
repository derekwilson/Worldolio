namespace MauiPOC.Views;

public partial class PlanPage : ContentPage
{
	public PlanPage()
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

        this.Window.Title = "Worldolio - Time Planner";
    }
}