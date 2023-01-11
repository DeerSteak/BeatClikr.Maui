namespace BeatClikr.Maui.Views;

public partial class SongDetailsPage : ContentPage
{
	public SongDetailsPage(ViewModels.SongDetailsViewModel songDetailsViewModel)
	{
		InitializeComponent();
		BindingContext = songDetailsViewModel;
	}
}
