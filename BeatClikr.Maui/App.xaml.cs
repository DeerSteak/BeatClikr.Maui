using BeatClikr.Maui.ViewModels;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
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
    //AppCenter.Start("ios=\"fb97bcfa-abf2-4528-adaf-3a84cc92a357\";" +
    //              "android=\"e2635483-a1e8-47c5-b57f-5ae2c50be4d1\";" +
    //              typeof(Analytics), typeof(Crashes));

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
  }

  protected override void OnSleep()
  {
    var deviceDisplay = ServiceHelper.GetService<IDeviceDisplay>();
    if (deviceDisplay != null)
      deviceDisplay.KeepScreenOn = false;
    var metronome = ServiceHelper.GetService<MetronomeClickerViewModel>();
    if (metronome != null)
      metronome.StopCommand.Execute(null);
    base.OnSleep();
  }

  public static void SetupAdmob()
  {
    var usePersonalizedAds = true;
#if IOS
    var trackingService = ServiceHelper.GetService<Services.Interfaces.IAdTrackingHandlerService>();
    usePersonalizedAds = trackingService.AskTrackingPermission().Result;
#endif

#if IOS || ANDROID
    CrossMauiMTAdmob.Current.UserPersonalizedAds = usePersonalizedAds;
    CrossMauiMTAdmob.Current.ComplyWithFamilyPolicies = true;
    CrossMauiMTAdmob.Current.UseRestrictedDataProcessing = true;
    CrossMauiMTAdmob.Current.TagForChildDirectedTreatment = MTTagForChildDirectedTreatment.TagForChildDirectedTreatmentUnspecified;
    CrossMauiMTAdmob.Current.TagForUnderAgeOfConsent = MTTagForUnderAgeOfConsent.TagForUnderAgeOfConsentUnspecified;
    CrossMauiMTAdmob.Current.MaxAdContentRating = MTMaxAdContentRating.MaxAdContentRatingG;
#endif
  }
}

