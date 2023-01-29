﻿namespace BeatClikr.Maui.Views;

public partial class AppShell : Shell
{
    bool askFlashlight;

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
        if (!Preferences.ContainsKey(PreferenceKeys.AskFlashlight))
        {
            askFlashlight = true;
        }
    }
}

