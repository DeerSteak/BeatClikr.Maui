namespace BeatClikr.Maui.Views;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(RouteNames.AboutPage, typeof(AboutPage));
        Routing.RegisterRoute(RouteNames.AppShell, typeof(AppShell));
        Routing.RegisterRoute(RouteNames.HelpPage, typeof(HelpPage));
        Routing.RegisterRoute(RouteNames.LibraryPage, typeof(LibraryPage));
        Routing.RegisterRoute(RouteNames.LivePage, typeof(LivePage));
        Routing.RegisterRoute(RouteNames.MainPage, typeof(MainPage));
        Routing.RegisterRoute(RouteNames.MetronomePage, typeof(MetronomePage));
        Routing.RegisterRoute(RouteNames.RehearsalPage, typeof(RehearsalPage));
        Routing.RegisterRoute(RouteNames.SettingsPage, typeof(SettingsPage));
        Routing.RegisterRoute(RouteNames.SongDetailsPage, typeof(SongDetailsPage));
        Routing.RegisterRoute(RouteNames.GetStartedPage, typeof(GetStartedPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!Preferences.ContainsKey(PreferenceKeys.UsePersonalizedAds))
        {
            GoToAsync($"/{RouteNames.GetStartedPage}");
        }
    }
}

