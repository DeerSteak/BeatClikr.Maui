
namespace BeatClikr.Maui.Views;

public partial class LivePage : ContentPage
{
	public LivePage(ViewModels.LiveViewModel liveViewModel)
	{
		InitializeComponent();
		liveViewModel.InitSongs();
        Disappearing += (s, e) => liveViewModel.StopCommand.Execute(null);
        BindingContext = liveViewModel;
	}

	public LivePage() : this(ServiceHelper.GetService<ViewModels.LiveViewModel>())
	{

	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
