using BeatClikr.Maui.ViewModels;

namespace BeatClikr.Maui.Views;

public partial class HelpPage : ContentPage
{
	public HelpPage(HelpViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
