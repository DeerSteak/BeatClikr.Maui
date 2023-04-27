using BeatClikr.Maui.Models;
using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels;

public partial class MetronomeClickerViewModel : ObservableObject
{
    private IShellService _shellService;
    private readonly string _bulbDim;
    private readonly string _bulbLit;
    private IMetronomeService _metronome;
    private IDeviceInfo _deviceInfo;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private bool _isLiveMode;
    partial void OnIsLiveModeChanged(bool value)
    {
        IMetronomeService.LiveMode = value;
    }

    [ObservableProperty]
    private string _beatBox;

    [ObservableProperty]
    private bool _muteOverride;
    partial void OnMuteOverrideChanged(bool value)
    {
        IsSilent = value;
    }

    [ObservableProperty]
    private bool _useFlashlight;
    partial void OnUseFlashlightChanged(bool value)
    {
        if (!value)
            Task.Run(async () => await Flashlight.Default.TurnOffAsync());
    }

    [ObservableProperty]
    private ClickerBeatType _beatType;
    partial void OnBeatTypeChanged(ClickerBeatType value)
    {
        SetupMetronome();
    }

    [ObservableProperty]
    private Song _song;

    [ObservableProperty]
    private bool _isSilent;

    public MetronomeClickerViewModel(IAppInfo appInfo, IShellService shellService, IMetronomeService metronome, IDeviceInfo deviceInfo)
    {
        _shellService = shellService;
        _metronome = metronome;
        _deviceInfo = deviceInfo;
        var currentTheme = appInfo.RequestedTheme;

        MuteOverride = Preferences.Get(PreferenceKeys.MuteMetronome, false);
        UseFlashlight = Preferences.Get(PreferenceKeys.UseFlashlight, true);

        _bulbDim = Constants.IconFont.Lightbulb;

        _bulbLit = Constants.IconFont.LightbulbOn;

        BeatBox = _bulbDim;
        Song = new Song();

        IMetronomeService.BeatAction = BeatAction;
        IMetronomeService.RhythmAction = RhythmAction;
    }

    private void BeatAction()
    {
        BeatBox = _bulbLit;
        if (UseFlashlight && _deviceInfo.Platform == DevicePlatform.Android)
            Task.Run(() => Flashlight.Default.TurnOnAsync().Start());
    }

    private void RhythmAction()
    {
        BeatBox = _bulbDim;
        if (UseFlashlight && _deviceInfo.Platform == DevicePlatform.Android)
            Task.Run(() => Flashlight.Default.TurnOffAsync().Start());
    }

    [RelayCommand]
    private void SetSong(Song song)
    {
        var wasPlaying = IsPlaying;
        Stop();
        this.Song = song;
        SetupMetronome();
        if (wasPlaying)
            _metronome.Play();
    }

    [RelayCommand]
    private async void StartStop()
    {
        if (!IsPlaying)
        {
            await PermissionsHelper.AskAllPermissions().ConfigureAwait(true);
            SetupMetronome();
        }            
        IsPlaying = !IsPlaying;
        if (IsPlaying)
            _metronome.Play();
        else
            Stop();
    }

    [RelayCommand]
    private void Stop()
    {
        IsPlaying = false;
        _metronome.Stop();
        RhythmAction();
    }

    [RelayCommand]
    private void SetupMetronome()
    {
        var mute = Preferences.Get(PreferenceKeys.MuteMetronome, false);
        IMetronomeService.MuteOverride = mute;

        string rhythm = string.Empty;
        string beat = string.Empty;

        switch (BeatType)
        {
            case ClickerBeatType.Live:
                rhythm = Preferences.Get(PreferenceKeys.LiveRhythm, FileNames.ClickLo);
                beat = Preferences.Get(PreferenceKeys.LiveBeat, FileNames.ClickHi);
                break;
            case ClickerBeatType.Rehearsal:
                rhythm = Preferences.Get(PreferenceKeys.RehearsalRhythm, FileNames.ClickLo);
                beat = Preferences.Get(PreferenceKeys.RehearsalBeat, FileNames.ClickHi);
                break;
            default:
                rhythm = Preferences.Get(PreferenceKeys.InstantRhythm, FileNames.ClickLo);
                beat = Preferences.Get(PreferenceKeys.InstantBeat, FileNames.ClickHi);
                break;
        }

        var numSubdivisions = 1;
        switch (Song.Subdivision)
        {
            case SubdivisionEnum.Eighth:
                numSubdivisions = 2;
                break;
            case SubdivisionEnum.TripletEighth:
                numSubdivisions = 3;
                break;
            case SubdivisionEnum.Sixteenth:
                numSubdivisions = 4;
                break;
            default: //Quarter note default
                numSubdivisions = 1;
                break;
        }

        _metronome.SetupMetronome(beat, rhythm, FileNames.Set1);
        _metronome.SetTempo(Song.BeatsPerMinute, numSubdivisions);
    }
}

