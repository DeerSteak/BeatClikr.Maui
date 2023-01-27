﻿global using BeatClikr.Maui.Constants;
global using BeatClikr.Maui.Enums;
global using BeatClikr.Maui.Helpers;
global using BeatClikr.Maui.Services;
using System.Reflection;
using CommunityToolkit.Maui;
using Plugin.Maui.Audio;
using Plugin.MauiMTAdmob;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

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
			.UseMauiMTAdmob()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("FontAwesome6Pro-Regular-400.otf", "FARegular");
			});

		return builder.Build();
	}

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
		var viewModels = GetTypesForNamespace("BeatClikr.Maui.ViewModels");

		foreach (var vm in viewModels)
			mauiAppBuilder.Services.AddSingleton(vm);

        return mauiAppBuilder;
    }

	public static MauiAppBuilder RegisterAppServices(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<IDeviceInfo>(DeviceInfo.Current);
		mauiAppBuilder.Services.AddSingleton<IAppInfo>(AppInfo.Current);
		mauiAppBuilder.Services.AddSingleton<Services.Interfaces.IShellService, Services.ShellService>();
		mauiAppBuilder.Services.AddSingleton<Services.Interfaces.IDataService, Services.DataService>();
		mauiAppBuilder.Services.AddSingleton<IAudioManager>(AudioManager.Current);
		mauiAppBuilder.Services.AddSingleton<Services.Interfaces.IMetronome, BeatClikr.Maui.Platforms.Metronome>();
		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
	{
        var views = GetTypesForNamespace("BeatClikr.Maui.Views");

        foreach (var v in views)
            mauiAppBuilder.Services.AddSingleton(v);

        return mauiAppBuilder;
	}

	private static System.Type[] GetTypesForNamespace(string nameSpace)
	{
        System.Type[] theList = Assembly.GetExecutingAssembly().GetTypes()
                                      .Where(x => !string.IsNullOrEmpty(x.Namespace))
                                      .Where(x => x.IsClass && x.IsPublic)
                                      .Where(x => x.Namespace.StartsWith(nameSpace)).ToArray();

        return theList;
    }
}

