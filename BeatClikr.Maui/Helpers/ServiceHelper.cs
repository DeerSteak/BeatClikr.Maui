namespace BeatClikr.Maui.Helpers;

public static class ServiceHelper
{
    public static TService GetService<TService>() => Current.GetService<TService>();

    public static IServiceProvider Current => IPlatformApplication.Current.Services;
// #if IOS || MACCATALYST
//             MauiUIApplicationDelegate.Current.Services;
// #elif ANDROID
//             MauiApplication.Current.Services;
// #else
//             null;
// #endif
}

