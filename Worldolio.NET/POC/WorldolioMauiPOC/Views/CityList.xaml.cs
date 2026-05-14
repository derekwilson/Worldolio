namespace WorldolioMauiPOC.Views;

public partial class CityList : ContentPage
{
	public CityList()
	{
		InitializeComponent();
        // Initializing the BindingContext with Products
        BindingContext = new Models.CityList();
    }

    private void citiesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}