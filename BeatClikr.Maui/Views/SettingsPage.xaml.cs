namespace BeatClikr.Maui.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(ViewModels.SettingsViewModel settingsViewModel)
    {
        InitializeComponent();
        BindingContext = settingsViewModel;
        AdView.AdsId = DeviceInfo.Platform == DevicePlatform.Android ? "ca-app-pub-8377432895177958/3979175164" : "ca-app-pub-8377432895177958/5175872615";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.SettingsViewModel).Init();
        AnalyticsHelper.TrackEvent($"{GetType()} appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        AnalyticsHelper.TrackEvent($"{GetType()} disappearing");
    }
}
