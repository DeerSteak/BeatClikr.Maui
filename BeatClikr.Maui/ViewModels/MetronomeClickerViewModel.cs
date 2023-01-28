using System.Timers;
using BeatClikr.Maui.Models;
using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;

namespace BeatClikr.Maui.ViewModels;

public partial class MetronomeClickerViewModel : ObservableObject
{
    private System.Timers.Timer _timer;
    private IShellService _shellService;
    private int _subdivisionNumber;
    private int _beatsPlayed = 0;
    private readonly ImageSource _bulbDim;
    private readonly ImageSource _bulbLit;
    private IMetronomeService _metronome;

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private bool _isLiveMode;

    [ObservableProperty]
    private ImageSource _beatBox;

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
    private bool _askFlashlight;

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

    public MetronomeClickerViewModel(IAudioManager audioManager, IAppInfo appInfo, IShellService shellService, IMetronomeService metronome)
    {
        _shellService = shellService;
        _metronome = metronome;

        var currentTheme = appInfo.RequestedTheme;

        MuteOverride = Preferences.Get(PreferenceKeys.MuteMetronome, false);
        AskFlashlight = Preferences.Get(PreferenceKeys.AskFlashlight, true);
        UseFlashlight = Preferences.Get(PreferenceKeys.UseFlashlight, true);

        _bulbDim = new FontImageSource()
        {
            FontFamily = "FARegular",
            Glyph = Constants.IconFont.Lightbulb,
            Color = appInfo.RequestedTheme == AppTheme.Dark ? Color.FromArgb("#FAFAFA") : Color.FromArgb("#212121"),
            Size = 90
        };

        _bulbLit = new FontImageSource()
        {
            FontFamily = "FARegular",
            Glyph = Constants.IconFont.LightbulbOn,
            Color = appInfo.RequestedTheme == AppTheme.Dark ? Color.FromArgb("#FAFAFA") : Color.FromArgb("#212121"),
            Size = 90
        };

        BeatBox = _bulbDim;
        Song = new Song();

        IMetronomeService.BeatAction = BeatImage;
        IMetronomeService.RhythmAction = RhythmImage;
    }

    private void BeatImage()
    {
        BeatBox = _bulbLit;
        if (UseFlashlight)
            Task.Run(() => Flashlight.Default.TurnOnAsync().Start());
    }

    private void RhythmImage()
    {
        BeatBox = _bulbDim;
        if (UseFlashlight)
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
    private void StartStop()
    {
        IsPlaying = !IsPlaying;
        if (IsPlaying)
            _metronome.Play();
        else
            _metronome.Stop();
    }

    [RelayCommand]
    private void Stop()
    {
        if (IsPlaying)
            _metronome.Stop();
    }

    [RelayCommand]
    private void SetupMetronome()
    {
        Task.Run(async () =>
        {
            var result = await Permissions.CheckStatusAsync<Permissions.Flashlight>();
            if (result != PermissionStatus.Granted)
            {
                var response = await _shellService.DisplayAlert("Flashlight Permission", "BeatClikr can use the flashlight on your device to show the beat. If you'd like to do this, press OK", "OK", "Cancel");
                if (response)
                {
                    result = await Permissions.RequestAsync<Permissions.Flashlight>();
                    if (result != PermissionStatus.Granted)
                    {
                        await _shellService.DisplayAlert("Flashlight Permission Denied", "BeatClikr will not use the flashlight. If you change your mind, you can enable the flashlight again on the Settings page.", "OK");
                    }
                }
                else
                {
                    AskFlashlight = false;
                    Preferences.Set(PreferenceKeys.AskFlashlight, AskFlashlight);
                }
            }

            UseFlashlight = result == PermissionStatus.Granted;
        });        

        string rhythm = string.Empty;
        string beat = string.Empty;

        switch (BeatType)
        {
            case ClickerBeatType.Live:
                rhythm = Preferences.Get(PreferenceKeys.LiveRhythm, FileNames.HatClosed);
                beat = Preferences.Get(PreferenceKeys.LiveBeat, FileNames.Kick);
                break;
            case ClickerBeatType.Rehearsal:
                rhythm = Preferences.Get(PreferenceKeys.RehearsalRhythm, FileNames.HatClosed);
                beat = Preferences.Get(PreferenceKeys.RehearsalBeat, FileNames.Kick);
                break;
            default:
                rhythm = Preferences.Get(PreferenceKeys.InstantRhythm, FileNames.HatClosed);
                beat = Preferences.Get(PreferenceKeys.InstantBeat, FileNames.Kick);
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
            default:
                break;
        }

        _metronome.SetupMetronome(beat, rhythm, FileNames.Set1);

        _metronome.SetTempo(Song.BeatsPerMinute, numSubdivisions);
    }       
}

