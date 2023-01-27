namespace BeatClikr.Maui.Views;

public partial class RehearsalPage : ContentPage
{
	public RehearsalPage(ViewModels.RehearsalViewModel rehearsalViewModel)
	{
		InitializeComponent();
        rehearsalViewModel.Init();
        Disappearing += (s, e) => rehearsalViewModel.StopCommand.Execute(null);
        Appearing += (s, e) => rehearsalViewModel.Init();
        BindingContext = rehearsalViewModel;
    }

    public RehearsalPage() : this(ServiceHelper.GetService<ViewModels.RehearsalViewModel>())
    {

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.RehearsalViewModel).Init();
    }
}
