using WorldolioMauiPOC.Data;

namespace WorldolioMauiPOC.Views;

public partial class CityList : ContentPage
{
	public CityList()
	{
		InitializeComponent();
        BindingContext = new Models.CityList();
    }

    private void citiesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}