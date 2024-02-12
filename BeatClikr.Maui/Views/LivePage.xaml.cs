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

    public LivePage() : this(IPlatformApplication.Current.Services.GetService<ViewModels.LiveViewModel>())
    {

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.LiveViewModel).Init();
    }
}
