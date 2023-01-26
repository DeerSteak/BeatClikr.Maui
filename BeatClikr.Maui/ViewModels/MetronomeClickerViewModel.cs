﻿using BeatClikr.Maui.Models;
using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;
using System.Timers;

namespace BeatClikr.Maui.ViewModels;

public partial class MetronomeClickerViewModel : ObservableObject
{
    private System.Timers.Timer _timer;
    private IAudioPlayer _playerBeat;
    private IAudioPlayer _playerRhythm;
    private IShellService _shellService;
    private int _subdivisionNumber;
    private int _beatsPlayed = 0;
    private readonly ImageSource _bulbDim;
    private readonly ImageSource _bulbLit;
    private bool _playSubdivisions;
    private readonly bool _onApple;
    private readonly IAudioManager _audioManager;

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
        SetSoundsAndLight(FileNames.Set1);
    }

    [ObservableProperty]
    private Song _song;

    [ObservableProperty]
    private bool _isSilent;

    public MetronomeClickerViewModel(IAudioManager audioManager, IAppInfo appInfo, IShellService shellService)
    {
        _audioManager = audioManager;
        _shellService = shellService;
        _onApple = DeviceInfo.Platform == DevicePlatform.iOS
            || DeviceInfo.Platform == DevicePlatform.MacCatalyst;

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
    }

    [RelayCommand]
    private void SetSong(Song song)
    {
        var wasPlaying = IsPlaying;
        if (IsPlaying)
            StopSongMetronome();
        this.Song = song;
        if (wasPlaying)
            StartStop();
    }

    [RelayCommand]
    private void StartStop()
    {
        IsPlaying = !IsPlaying;
        if (IsPlaying)
            PlaySongMetronome();
        else
            StopSongMetronome();
    }

    [RelayCommand]
    private void Stop()
    {
        if (IsPlaying)
            StopSongMetronome();
    }

    private void PlaySongMetronome()
    {
        _beatsPlayed = 0;
        IsSilent = MuteOverride;
        float timerInterval = PlaybackUtilities.GetTimerInterval(Song.Subdivision, Song.BeatsPerMinute);
        _playSubdivisions = Song.Subdivision != SubdivisionEnum.Quarter;
        _subdivisionNumber = 0;
        _timer = new System.Timers.Timer(timerInterval) { AutoReset = true };
        _timer.Elapsed += OnTimerElapsed;
        _timer.Enabled = true;
    }

    private void StopSongMetronome()
    {
        _timer.Enabled = false;
        BeatBox = _bulbDim;
        IsPlaying = false;
        IsSilent = false;
        Flashlight.Default.TurnOffAsync().Wait();
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {

        if (IsLiveMode && !IsSilent && _subdivisionNumber == 0)
        {
            if (_beatsPlayed >= Song.BeatsPerMeasure)
                IsSilent = true;
        }

        if (_subdivisionNumber == 0)
            BeatBox = _bulbLit;
        else
            BeatBox = _bulbDim;

        if (_subdivisionNumber == 0)
        {
            _beatsPlayed++;
            if (UseFlashlight)
                Task.Run(() => Flashlight.Default.TurnOnAsync().Start());
            if (!IsSilent && !MuteOverride)
                PlayBeat();
        }
        else
        {
            if (UseFlashlight)
                Task.Run(() => Flashlight.Default.TurnOffAsync().Start());
            if (_playSubdivisions && !IsSilent && !MuteOverride)
                PlayRhythm();
        }

        _subdivisionNumber++;
        if (_subdivisionNumber >= PlaybackUtilities.GetSubdivisionsPerBeat(Song.Subdivision))
            _subdivisionNumber = 0;
    }

    private void PlayRhythm()
    {
        if (_onApple)
            _playerRhythm.Stop();
        else
            _playerRhythm.Seek(0);
        _playerRhythm.Play();
    }

    private void PlayBeat()
    {
        if (_onApple)
            _playerBeat.Stop();
        else
            _playerBeat.Seek(0);
        _playerBeat.Play();
    }

    private async void SetSoundsAndLight(string set)
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

        string rhythm = string.Empty;
        string beat = string.Empty;

        switch (BeatType)
        {
            case ClickerBeatType.Live:
                rhythm = Preferences.Get(PreferenceKeys.LiveRhythm, FileNames.HatClosed);
                beat = Preferences.Get(PreferenceKeys.LiveBeat, FileNames.Kick);
                break;
            case ClickerBeatType.Instant:
                rhythm = Preferences.Get(PreferenceKeys.LiveRhythm, FileNames.HatClosed);
                beat = Preferences.Get(PreferenceKeys.LiveBeat, FileNames.Kick);
                break;
            case ClickerBeatType.Rehearsal:
                rhythm = Preferences.Get(PreferenceKeys.LiveRhythm, FileNames.HatClosed);
                beat = Preferences.Get(PreferenceKeys.LiveBeat, FileNames.Kick);
                break;
            default:
                rhythm = FileNames.HatClosed;
                beat = FileNames.Kick;
                break;
        }

        _playerRhythm = _audioManager.CreatePlayer(PlaybackUtilities.GetStreamFromFile(rhythm, set).Result);
        _playerBeat = _audioManager.CreatePlayer(PlaybackUtilities.GetStreamFromFile(beat, set).Result);
    }
}

