using BeatClikr.Maui.ViewModels;

namespace BeatClikr.Maui.Views;

public partial class RehearsalPage : ContentPage
{
	public RehearsalPage(RehearsalViewModel rehearsalViewModel)
	{
		InitializeComponent();
        rehearsalViewModel.InitSongs();
        Disappearing += (s, e) => rehearsalViewModel.StopCommand.Execute(null);
        Appearing += (s, e) => rehearsalViewModel.InitSongs();
        BindingContext = rehearsalViewModel;
    }

    public RehearsalPage() : this(ServiceHelper.GetService<RehearsalViewModel>())
    {

    }
}
