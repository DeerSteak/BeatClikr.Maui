using Plugin.MauiMTAdmob;
using Plugin.MauiMTAdmob.Extra;

namespace BeatClikr.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        SetupAdmob();
        MainPage = ServiceHelper.GetService<Views.AppShell>();        
    }

    protected override void OnStart()
    {
        base.OnStart();

        if (!Preferences.ContainsKey(PreferenceKeys.InstantBeat))
            Preferences.Set(PreferenceKeys.InstantBeat, FileNames.ClickHi);

        if (!Preferences.ContainsKey(PreferenceKeys.InstantRhythm))
            Preferences.Set(PreferenceKeys.InstantRhythm, FileNames.ClickLo);

        if (!Preferences.ContainsKey(PreferenceKeys.RehearsalBeat))
            Preferences.Set(PreferenceKeys.RehearsalBeat, FileNames.ClickHi);

        if (!Preferences.ContainsKey(PreferenceKeys.RehearsalRhythm))
            Preferences.Set(PreferenceKeys.RehearsalRhythm, FileNames.ClickLo);

        if (!Preferences.ContainsKey(PreferenceKeys.LiveBeat))
            Preferences.Set(PreferenceKeys.LiveBeat, FileNames.ClickHi);

        if (!Preferences.ContainsKey(PreferenceKeys.LiveRhythm))
            Preferences.Set(PreferenceKeys.LiveRhythm, FileNames.ClickLo);

        if (!Preferences.ContainsKey(PreferenceKeys.UsePersonalizedAds))
            Preferences.Set(PreferenceKeys.UsePersonalizedAds, true);
    }

    private void SetupAdmob()
    {
        var useAds = Preferences.Get(PreferenceKeys.UsePersonalizedAds, true);
#if IOS || ANDROID
        CrossMauiMTAdmob.Current.UserPersonalizedAds = useAds;
        if (useAds)
            CrossMauiMTAdmob.Current.AdsId = DeviceInfo.Platform == DevicePlatform.iOS
                ? "ca-app-pub-8377432895177958/7490720167"
                : "ca-app-pub-8377432895177958/9298625858";
        CrossMauiMTAdmob.Current.ComplyWithFamilyPolicies = true;
        CrossMauiMTAdmob.Current.UseRestrictedDataProcessing = true;
        CrossMauiMTAdmob.Current.TestDevices = new List<string>() { };
        CrossMauiMTAdmob.Current.TagForChildDirectedTreatment = MTTagForChildDirectedTreatment.TagForChildDirectedTreatmentUnspecified;
        CrossMauiMTAdmob.Current.TagForUnderAgeOfConsent = MTTagForUnderAgeOfConsent.TagForUnderAgeOfConsentUnspecified;
        CrossMauiMTAdmob.Current.MaxAdContentRating = MTMaxAdContentRating.MaxAdContentRatingG;
#endif
    }
}

