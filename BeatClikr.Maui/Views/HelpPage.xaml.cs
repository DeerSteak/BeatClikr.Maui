namespace BeatClikr.Maui.Views;

public partial class HelpPage : ContentPage
{
	public HelpPage(ViewModels.HelpViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	public HelpPage() : this(ServiceHelper.GetService<ViewModels.HelpViewModel>())
	{

	}
}
