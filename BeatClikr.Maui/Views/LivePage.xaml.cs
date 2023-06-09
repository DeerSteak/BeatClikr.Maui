using Microsoft.AppCenter.Analytics;

namespace BeatClikr.Maui.Views;

public partial class LivePage : ContentPage
{
    public LivePage(ViewModels.LiveViewModel liveViewModel)
    {
        InitializeComponent();
        AdView.AdsId = DeviceInfo.Platform == DevicePlatform.Android ? "ca-app-pub-8377432895177958/5497900071" : "ca-app-pub-8377432895177958/9628716771";
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
        Analytics.TrackEvent($"{GetType()} appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Analytics.TrackEvent($"{GetType()} disappearing");
    }
}
