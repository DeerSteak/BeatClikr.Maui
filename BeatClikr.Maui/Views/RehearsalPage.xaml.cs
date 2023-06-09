using Microsoft.AppCenter.Analytics;

namespace BeatClikr.Maui.Views;

public partial class RehearsalPage : ContentPage
{
    public RehearsalPage(ViewModels.RehearsalViewModel rehearsalViewModel)
    {
        InitializeComponent();
        AdView.AdsId = DeviceInfo.Platform == DevicePlatform.Android ? "ca-app-pub-8377432895177958/8149195717" : "ca-app-pub-8377432895177958/8507206795";
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
        Analytics.TrackEvent($"{GetType()} appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Analytics.TrackEvent($"{GetType()} disappearing");
    }
}
