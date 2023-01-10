global using BeatClikr.Maui.Constants;
global using BeatClikr.Maui.Enums;
global using BeatClikr.Maui.Helpers;
global using BeatClikr.Maui.Services;

using CommunityToolkit.Maui;
using Plugin.Maui.Audio;

namespace BeatClikr.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.RegisterAppServices()
			.RegisterViewModels()
			.RegisterViews()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		return builder.Build();
	}

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<ViewModels.AboutViewModel>();
        return mauiAppBuilder;
    }

	public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<Services.Interfaces.IShellService, Services.ShellService>();
		mauiAppBuilder.Services.AddSingleton<Services.Interfaces.IPersistence, Services.Persistence>();
		mauiAppBuilder.Services.AddSingleton<IAudioManager>(AudioManager.Current);
		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
	{
        mauiAppBuilder.Services.AddSingleton<Views.AboutPage>();
        mauiAppBuilder.Services.AddSingleton<Views.AppShell>();
        mauiAppBuilder.Services.AddSingleton<Views.HelpPage>();
        mauiAppBuilder.Services.AddSingleton<Views.LibraryPage>();
        mauiAppBuilder.Services.AddSingleton<Views.LivePage>();
        mauiAppBuilder.Services.AddSingleton<Views.MainPage>();
        mauiAppBuilder.Services.AddSingleton<Views.MetronomePage>();
        mauiAppBuilder.Services.AddSingleton<Views.RehearsalPage>();
        mauiAppBuilder.Services.AddSingleton<Views.SettingsPage>();
        mauiAppBuilder.Services.AddSingleton<Views.SongDetailsPage>();
        return mauiAppBuilder;
	}
}

