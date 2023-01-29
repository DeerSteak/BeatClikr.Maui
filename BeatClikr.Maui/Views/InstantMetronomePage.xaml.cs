﻿namespace BeatClikr.Maui.Views;

public partial class InstantMetronomePage : ContentPage
{
    public InstantMetronomePage(ViewModels.InstantMetronomeViewModel metronomeViewModel)
    {
        BindingContext = metronomeViewModel;
        InitializeComponent();
    }

    public InstantMetronomePage() : this(ServiceHelper.GetService<ViewModels.InstantMetronomeViewModel>())
    {

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.InstantMetronomeViewModel).Init();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var onboarded = Preferences.Get(PreferenceKeys.Onboarded, new DateTime(1900, 1, 1));
        if (onboarded < new DateTime(2023, 1, 29))
            Shell.Current.Navigation.PushModalAsync(ServiceHelper.GetService<Views.GetStartedPage>());
        //MainPage = new NavigationPage(ServiceHelper.GetService<Views.GetStartedPage>());
        //else
        //    MainPage = ServiceHelper.GetService<Views.AppShell>();
    }

    void Button_Clicked(object sender, EventArgs e)
    {
        (BindingContext as ViewModels.InstantMetronomeViewModel).PlayStopToggledCommand.Execute(null);
    }
}
