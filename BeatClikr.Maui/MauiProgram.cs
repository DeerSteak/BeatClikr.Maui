global using BeatClikr.Maui.Constants;
global using BeatClikr.Maui.Enums;
global using BeatClikr.Maui.Helpers;

using BeatClikr.Maui.Services;
using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Maui;
using Plugin.MauiMTAdmob;
using System.Reflection;

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
		mauiAppBuilder.Services.AddSingleton(DeviceInfo.Current);
		mauiAppBuilder.Services.AddSingleton(AppInfo.Current);
		mauiAppBuilder.Services.AddSingleton<IShellService, ShellService>();
		mauiAppBuilder.Services.AddSingleton<IDataService, DataService>();
#if IOS
		mauiAppBuilder.Services.AddSingleton<IMetronomeService, BeatClikr.Maui.Platforms.iOS.MetronomeService>();
#elif ANDROID
		mauiAppBuilder.Services.AddSingleton<IMetronomeService, BeatClikr.Maui.Platforms.Android.MetronomeService>();
#endif
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
		Type[] theList = Assembly.GetExecutingAssembly().GetTypes()
							.Where(x => !string.IsNullOrEmpty(x.Namespace))
							.Where(x => x.IsClass && x.IsPublic)
							.Where(x => x.Namespace.StartsWith(nameSpace)).ToArray();

        return theList;
    }
}

