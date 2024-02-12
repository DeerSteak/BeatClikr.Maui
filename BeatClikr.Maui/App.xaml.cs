using BeatClikr.Maui.ViewModels;

namespace BeatClikr.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = IPlatformApplication.Current.Services.GetService<Views.AppShell>();
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

    }

    protected override void OnSleep()
    {
        var deviceDisplay = IPlatformApplication.Current.Services.GetService<IDeviceDisplay>();
        if (deviceDisplay != null)
            deviceDisplay.KeepScreenOn = false;
        var metronome = IPlatformApplication.Current.Services.GetService<MetronomeClickerViewModel>();
        metronome?.StopCommand.Execute(null);
        base.OnSleep();
    }
}

