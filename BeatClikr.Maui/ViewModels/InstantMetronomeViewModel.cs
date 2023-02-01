using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BeatClikr.Maui.Models;

namespace BeatClikr.Maui.ViewModels;

public partial class InstantMetronomeViewModel : ObservableObject
{
    private SubdivisionEnum _subdivision;
    private const int _numBeats = 4;
    private MetronomeClickerViewModel _metronomeClickerViewModel;

    [ObservableProperty]
    private int _selectedSubdivisionIndex;
    partial void OnSelectedSubdivisionIndexChanged(int value)
    {
        switch (value)
        {
            case 0:
                _subdivision = SubdivisionEnum.Quarter;
                break;
            case 1:
                _subdivision = SubdivisionEnum.Eighth;
                break;
            case 2:
                _subdivision = SubdivisionEnum.TripletEighth;
                break;
            case 3:
                _subdivision = SubdivisionEnum.Sixteenth;
                break;
            default:
                _subdivision = SubdivisionEnum.Eighth;
                break;
        }
        SetupMetronome();
    }

    [ObservableProperty]
    private int _beatsPerMinute;

    [ObservableProperty]
    private string[] _subdivisions = new string[] { "Quarter", "Eighth", "Eighth Triplet", "Sixteenth" };

    [ObservableProperty]
    List<InstrumentPicker> _rhythmInstruments;

    [ObservableProperty]
    List<InstrumentPicker> _beatInstruments;

    [ObservableProperty]
    private InstrumentPicker _instantBeat;
    partial void OnInstantBeatChanged(InstrumentPicker value)
    {
        Preferences.Set(PreferenceKeys.InstantBeat, value.FileName);
        SetupMetronome();
    }

    [ObservableProperty]
    private InstrumentPicker _instantRhythm;
    partial void OnInstantRhythmChanged(InstrumentPicker value)
    {
        Preferences.Set(PreferenceKeys.InstantRhythm, value.FileName);
        SetupMetronome();
    }

    public InstantMetronomeViewModel(MetronomeClickerViewModel metronomeClickerViewModel, IShellService shellService)
    {
        _subdivision = SubdivisionEnum.Quarter;
        _metronomeClickerViewModel = metronomeClickerViewModel;
        _shellService = shellService;

        BeatsPerMinute = Preferences.Get(PreferenceKeys.InstantBpm, 60);
        SelectedSubdivisionIndex = Preferences.Get(PreferenceKeys.InstantSelectedSubdivisionIndex, 0);
        RhythmInstruments = InstrumentPicker.Instruments.Where(x => x.IsRhythm).ToList();
        BeatInstruments = InstrumentPicker.Instruments.Where(x => x.IsBeat).ToList();

        SetupMetronome();
    }

    [ObservableProperty]
    private bool _wasPlaying;
    private readonly IShellService _shellService;

    public void Init()
    {
        var instantBeatName = Preferences.Get(PreferenceKeys.InstantBeat, FileNames.Kick);
        InstantBeat = InstrumentPicker.FromString(instantBeatName);

        var instantRhythmName = Preferences.Get(PreferenceKeys.InstantRhythm, FileNames.HatClosed);
        InstantRhythm = InstrumentPicker.FromString(instantRhythmName);

        _metronomeClickerViewModel.BeatType = ClickerBeatType.Instant;
        _metronomeClickerViewModel.IsLiveMode = false;
        _metronomeClickerViewModel.SetupMetronomeCommand.Execute(null);
    }

    [RelayCommand]
    private void PlayStopToggled()
    {
        _metronomeClickerViewModel.StartStopCommand.Execute(null);
        WasPlaying = _metronomeClickerViewModel.IsPlaying;
    }

    [RelayCommand]
    private void Stop()
    {
        _metronomeClickerViewModel.StopCommand.Execute(null);
        WasPlaying = false;
    }

    [RelayCommand]
    private void SliderDragStarted()
    {
        WasPlaying = _metronomeClickerViewModel.IsPlaying;
        _metronomeClickerViewModel.StopCommand.Execute(null);
    }

    [RelayCommand]
    private void SliderDragCompleted()
    {
        Preferences.Set(PreferenceKeys.InstantBpm, BeatsPerMinute);
        var playing = WasPlaying;
        SetupMetronome();
        if (playing)
            _metronomeClickerViewModel.StartStopCommand.Execute(null);
        WasPlaying = _metronomeClickerViewModel.IsPlaying;
    }

    private void SetupMetronome()
    {
        WasPlaying = _metronomeClickerViewModel.IsPlaying;
        _metronomeClickerViewModel.StopCommand.Execute(null);
        var song = new Models.Song()
        {
            Title = "Instant metronome",
            Artist = "BeatClikr",
            BeatsPerMeasure = _numBeats,
            BeatsPerMinute = this.BeatsPerMinute,
            Subdivision = _subdivision
        };
        _metronomeClickerViewModel.SetSongCommand.Execute(song);
        if (WasPlaying)
        {
            _metronomeClickerViewModel.StartStopCommand.Execute(null);
            WasPlaying = _metronomeClickerViewModel.IsPlaying;
        }
    }
}
