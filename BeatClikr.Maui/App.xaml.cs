namespace BeatClikr.Maui;

public partial class App : Application
{
	public App(Views.AppShell shell)
	{
		InitializeComponent();

		MainPage = shell;
	}

    protected override void OnStart()
    {
        base.OnStart();

        if (!Preferences.ContainsKey(PreferenceKeys.InstantBeat))
            Preferences.Set(PreferenceKeys.InstantBeat, FileNames.Kick);

        if (!Preferences.ContainsKey(PreferenceKeys.InstantRhythm))
            Preferences.Set(PreferenceKeys.InstantRhythm, FileNames.HatClosed);

        if (!Preferences.ContainsKey(PreferenceKeys.RehearsalBeat))
            Preferences.Set(PreferenceKeys.RehearsalBeat, FileNames.Kick);

        if (!Preferences.ContainsKey(PreferenceKeys.RehearsalRhythm))
            Preferences.Set(PreferenceKeys.RehearsalRhythm, FileNames.HatClosed);

        if (!Preferences.ContainsKey(PreferenceKeys.LiveBeat))
            Preferences.Set(PreferenceKeys.LiveBeat, FileNames.Kick);

        if (!Preferences.ContainsKey(PreferenceKeys.LiveRhythm))
            Preferences.Set(PreferenceKeys.LiveRhythm, FileNames.HatClosed);
    }
}

