global using BeatClikr.Maui.Constants;
global using BeatClikr.Maui.Enums;
global using BeatClikr.Maui.Helpers;
using System.Reflection;
using BeatClikr.Maui.Services;
using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Maui;
using MetroLog.MicrosoftExtensions;
using MetroLog.Operators;
using Plugin.LocalNotification;
using Plugin.MauiMTAdmob;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace BeatClikr.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.Logging.AddStreamingFileLogger(options =>
        {
            options.MinLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
            options.MaxLevel = Microsoft.Extensions.Logging.LogLevel.Critical;
            options.RetainDays = 7;
            options.FolderPath = Path.Combine(
                FileSystem.CacheDirectory,
                "BeatClikrLog");
        });

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .RegisterAppServices()
            .RegisterViewModels()
            .UseLocalNotification()
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
        mauiAppBuilder.Services.AddSingleton(LogOperatorRetriever.Instance);
        mauiAppBuilder.Services.AddSingleton(DeviceInfo.Current);
        mauiAppBuilder.Services.AddSingleton(AppInfo.Current);
        mauiAppBuilder.Services.AddSingleton(DeviceDisplay.Current);
        mauiAppBuilder.Services.AddSingleton(Vibration.Default);
        mauiAppBuilder.Services.AddSingleton(LocalNotificationCenter.Current);
        mauiAppBuilder.Services.AddSingleton(Flashlight.Default);
        mauiAppBuilder.Services.AddSingleton(Plugin.StoreReview.CrossStoreReview.Current);
        mauiAppBuilder.Services.AddSingleton<IShellService, ShellService>();
        mauiAppBuilder.Services.AddSingleton<IDataService, DataService>();
        mauiAppBuilder.Services.AddSingleton<INonShellNavProvider, NonShellNavProvider>();
        mauiAppBuilder.Services.AddSingleton<ILocalNotificationService, LocalNotificationService>();
        mauiAppBuilder.Services.AddSingleton<ISetupService, SetupService>();
#if IOS
        mauiAppBuilder.Services.AddSingleton<IMetronomeService, Platforms.iOS.MetronomeService>();
        mauiAppBuilder.Services.AddSingleton<IAdTrackingHandlerService, Platforms.iOS.AdTrackingHandlerService>();
#elif ANDROID
        mauiAppBuilder.Services.AddSingleton<IMetronomeService, Platforms.Android.MetronomeService>();
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

