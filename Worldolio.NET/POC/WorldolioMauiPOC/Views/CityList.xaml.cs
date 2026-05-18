namespace WorldolioMauiPOC.Views;

public partial class CityList : ContentPage
{
	public CityList()
	{
		InitializeComponent();
        BindingContext = new ViewModels.CityList();
    }

    private void citiesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}