namespace WorldolioMauiPOC.Views;

public partial class CityGrid : ContentPage
{
	public CityGrid(CityGridViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
    private void citiesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}