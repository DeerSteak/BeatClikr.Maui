using BeatClikr.Maui.Models;
using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AppCenter.Analytics;

namespace BeatClikr.Maui.ViewModels;

public partial class MetronomeClickerViewModel : ObservableObject
{
    private IShellService _shellService;
    private readonly string _bulbDim;
    private readonly string _bulbLit;
    private IMetronomeService _metronome;
    private IDeviceInfo _deviceInfo;
    private IDeviceDisplay _deviceDisplay;
    private IPermissionService _permissionService;

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

    [ObservableProperty]
    private Action<uint> _setBeatMilliseconds;

    [ObservableProperty]
    private Action _animate;

    [ObservableProperty]
    private Action _resetAnimator;

    public MetronomeClickerViewModel(IDeviceDisplay deviceDisplay, IAppInfo appInfo, IShellService shellService, IMetronomeService metronome, IDeviceInfo deviceInfo, IPermissionService permissionService)
    {
        _shellService = shellService;
        _metronome = metronome;
        _deviceInfo = deviceInfo;
        _deviceDisplay = deviceDisplay;
        _permissionService = permissionService;
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
        Animate?.Invoke();
        BeatBox = _bulbLit;
    }

    private void RhythmAction()
    {
        BeatBox = _bulbDim;
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
            await _permissionService.AskAllPermissions().ConfigureAwait(true);
            SetupMetronome();
        }
        IsPlaying = !IsPlaying;
        if (IsPlaying)
            Start();
        else
            Stop();
    }

    [RelayCommand]
    private void Stop()
    {
        ResetAnimator?.Invoke();
        IsPlaying = false;
        _metronome.Stop();
        RhythmAction();
        _deviceDisplay.KeepScreenOn = false;

        AnalyticsHelper.TrackEvent($"Metronome stopped");

    }

    [RelayCommand]
    private void Start()
    {
        _metronome.Play();
        _deviceDisplay.KeepScreenOn = true;

        AnalyticsHelper.TrackEvent($"Metronome started");
    }

    [RelayCommand]
    private void SetupMetronome()
    {
        var mute = Preferences.Get(PreferenceKeys.MuteMetronome, false);
        IMetronomeService.MuteOverride = mute;
        string beat;

        string rhythm;
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

        var numSubdivisions = Song.Subdivision switch
        {
            Subdivisions.Eighth => 2,
            Subdivisions.TripletEighth => 3,
            Subdivisions.Sixteenth => 4,
            Subdivisions.Quarter => 1,
            _ => 1,
        };
        _metronome.SetupMetronome(beat, rhythm, FileNames.Set1);
        _metronome.SetTempo(Song.BeatsPerMinute, numSubdivisions);
        SetBeatMilliseconds?.Invoke((uint)(_metronome.GetMillisecondsPerBeat()));
    }
}

