namespace BeatClikr.Maui.Views;

public partial class InstantMetronomePage : ContentPage
{
    public InstantMetronomePage(ViewModels.InstantMetronomeViewModel metronomeViewModel)
    {
        BindingContext = metronomeViewModel;
        InitializeComponent();
    }

    public InstantMetronomePage() : this(ServiceHelper.GetService<ViewModels.InstantMetronomeViewModel>()) { }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.InstantMetronomeViewModel).Init();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        var vm = BindingContext as ViewModels.InstantMetronomeViewModel;
        if (vm.WasPlaying)
            vm.StopCommand.Execute(null);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var onboarded = Preferences.Get(PreferenceKeys.Onboarded, new DateTime(1900, 1, 1));
        if (onboarded < new DateTime(2023, 1, 29))
            Shell.Current.Navigation.PushModalAsync(ServiceHelper.GetService<Views.GetStartedPage>());
    }
}
