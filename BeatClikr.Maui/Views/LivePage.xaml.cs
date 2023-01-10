using BeatClikr.Maui.ViewModels;

namespace BeatClikr.Maui.Views;

public partial class LivePage : ContentPage
{
	LiveViewModel _liveViewModel;
	public LivePage(LiveViewModel liveViewModel)
	{
		InitializeComponent();
		_liveViewModel = liveViewModel;
		BindingContext = _liveViewModel;
		PlaybackCheckbox.CheckedChanged += (s, e) => liveViewModel.PlaybackCheckboxChangedCommand.Execute(null);
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		_liveViewModel.InitSongs();
    }

    protected override void OnDisappearing()
    {
        base.OnAppearing();
		_liveViewModel.StopCommand.Execute(null);
    }
}
