using BeatClikr.Maui.ViewModels;

namespace BeatClikr.Maui.Views;

public partial class RehearsalPage : ContentPage
{
	public RehearsalPage(RehearsalViewModel rehearsalViewModel)
	{
		InitializeComponent();
        rehearsalViewModel.Init();
        Disappearing += (s, e) => rehearsalViewModel.StopCommand.Execute(null);
        Appearing += (s, e) => rehearsalViewModel.Init();
        BindingContext = rehearsalViewModel;
    }

    public RehearsalPage() : this(ServiceHelper.GetService<RehearsalViewModel>())
    {

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as RehearsalViewModel).Init();
    }
}
