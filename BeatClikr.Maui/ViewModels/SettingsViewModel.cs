using BeatClikr.Maui.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatClikr.Maui.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    List<InstrumentPicker> _rhythmInstruments;

    [ObservableProperty]
    List<InstrumentPicker> _beatInstruments;

    [ObservableProperty]
    private InstrumentPicker _instantBeat;
    partial void OnInstantBeatChanged(InstrumentPicker value)
    {
        Preferences.Set(PreferenceKeys.InstantBeat, value.FileName);
    }

    [ObservableProperty]
    private InstrumentPicker _instantRhythm;
    partial void OnInstantRhythmChanged(InstrumentPicker value)
    {
        Preferences.Set(PreferenceKeys.InstantRhythm, value.FileName);
    }

    [ObservableProperty]
    private InstrumentPicker _liveBeat;
    partial void OnLiveBeatChanged(InstrumentPicker value)
    {
        Preferences.Set(PreferenceKeys.LiveBeat, value.FileName);
    }

    [ObservableProperty]
    private InstrumentPicker _liveRhythm;
    partial void OnLiveRhythmChanged(InstrumentPicker value)
    {        
        Preferences.Set(PreferenceKeys.LiveRhythm, value.FileName);
    }

    [ObservableProperty]
    private InstrumentPicker _rehearsalBeat;
    partial void OnRehearsalBeatChanged(InstrumentPicker value)
    {
        Preferences.Set(PreferenceKeys.RehearsalBeat, value.FileName);
    }

    [ObservableProperty]
    private InstrumentPicker _rehearsalRhythm;
    partial void OnRehearsalRhythmChanged(InstrumentPicker value)
    {
        Preferences.Set(PreferenceKeys.RehearsalRhythm, value.FileName);
    }

    public SettingsViewModel()
    {
        RhythmInstruments = InstrumentPicker.Instruments.Where(x => x.IsRhythm).ToList();
        BeatInstruments = InstrumentPicker.Instruments.Where(x => x.IsBeat).ToList();
    }

    public void Init()
    {
        var instantBeatName = Preferences.Get(PreferenceKeys.InstantBeat, FileNames.Kick);
        InstantBeat = InstrumentPicker.FromString(instantBeatName);

        var instantRhythmName = Preferences.Get(PreferenceKeys.InstantRhythm, FileNames.HatClosed);
        InstantRhythm = InstrumentPicker.FromString(instantRhythmName);

        var liveBeatName = Preferences.Get(PreferenceKeys.LiveBeat, FileNames.Kick);
        LiveBeat = InstrumentPicker.FromString(liveBeatName);

        var liveRhythmName = Preferences.Get(PreferenceKeys.InstantRhythm, FileNames.HatClosed);
        LiveRhythm = InstrumentPicker.FromString(liveRhythmName);

        var rehearsalBeatName = Preferences.Get(PreferenceKeys.RehearsalBeat, FileNames.Kick);
        RehearsalBeat = InstrumentPicker.FromString(rehearsalBeatName);

        var rehearsalRhythmName = Preferences.Get(PreferenceKeys.RehearsalRhythm, FileNames.HatClosed);
        RehearsalRhythm = InstrumentPicker.FromString(rehearsalRhythmName);
    }
}

