using Microsoft.AppCenter.Analytics;

namespace BeatClikr.Maui.Views;

public partial class AppShell : Shell
{
    public AppShell(ViewModels.AppShellViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        Routing.RegisterRoute(RouteNames.AboutRoute, typeof(AboutPage));
        Routing.RegisterRoute(RouteNames.AppShellRoute, typeof(AppShell));
        Routing.RegisterRoute(RouteNames.HelpRoute, typeof(HelpPage));
        Routing.RegisterRoute(RouteNames.LibraryRoute, typeof(LibraryPage));
        Routing.RegisterRoute(RouteNames.LiveRoute, typeof(LivePage));
        Routing.RegisterRoute(RouteNames.InstantMetronomeRoute, typeof(InstantMetronomePage));
        Routing.RegisterRoute(RouteNames.RehearsalRoute, typeof(RehearsalPage));
        Routing.RegisterRoute(RouteNames.SettingsRoute, typeof(SettingsPage));
        Routing.RegisterRoute(RouteNames.SongDetailsRoute, typeof(SongDetailsPage));
        Routing.RegisterRoute(RouteNames.GetStartedRoute, typeof(GetStartedPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Analytics.TrackEvent($"{GetType()} appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Analytics.TrackEvent($"{GetType()} disappearing");
    }
}

