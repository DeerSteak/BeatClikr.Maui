namespace BeatClikr.Maui.Views;

public partial class LivePage : ContentPage
{
	public LivePage(ViewModels.LiveViewModel liveViewModel)
	{
		InitializeComponent();
        BindingContext = liveViewModel;
        Disappearing += (s, e) => liveViewModel.StopCommand.Execute(null);
		Appearing += (s, e) => liveViewModel.Init();
	}

	public LivePage() : this(ServiceHelper.GetService<ViewModels.LiveViewModel>())
	{

	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		(BindingContext as ViewModels.LiveViewModel).Init();
    }
}
