using Microsoft.AppCenter.Analytics;

namespace BeatClikr.Maui.Views;

public partial class InstantMetronomePage : ContentPage
{
    public InstantMetronomePage(ViewModels.InstantMetronomeViewModel metronomeViewModel)
    {
        BindingContext = metronomeViewModel;
        InitializeComponent();
        AdView.AdsId = DeviceInfo.Platform == DevicePlatform.Android ? "ca-app-pub-8377432895177958/9298625858" : "ca-app-pub-8377432895177958/7490720167";
    }

    public InstantMetronomePage() : this(ServiceHelper.GetService<ViewModels.InstantMetronomeViewModel>()) { }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.InstantMetronomeViewModel).Init();
        Analytics.TrackEvent($"{GetType()} appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        var vm = BindingContext as ViewModels.InstantMetronomeViewModel;
        if (vm.WasPlaying)
            vm.StopCommand.Execute(null);
        Analytics.TrackEvent($"{GetType()} disappearing");
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var onboarded = Preferences.Get(PreferenceKeys.Onboarded, new DateTime(1900, 1, 1));
        if (onboarded < new DateTime(2023, 1, 29))
            Shell.Current.Navigation.PushModalAsync(ServiceHelper.GetService<Views.GetStartedPage>());
    }
}
