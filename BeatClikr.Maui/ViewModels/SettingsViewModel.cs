using BeatClikr.Maui.Models;
using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Resources;

namespace BeatClikr.Maui.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    bool _sendReminders;
    partial void OnSendRemindersChanged(bool value)
    {
        Preferences.Set(PreferenceKeys.PracticeReminders, value);
        string msg;
        if (value) msg = "You will receive reminders daily, starting this time tomorrow";
        else msg = "Previously-scheduled notifications canceled.";
        
        var snackbarOptions = new SnackbarOptions
        { 
            BackgroundColor = Color.FromArgb("#408CC4"),
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.Black,
            CornerRadius = new CornerRadius(5)
        };

        var snackBar = Snackbar.Make(msg, null, "OK", TimeSpan.FromSeconds(5), snackbarOptions);
        snackBar.Show();
    }

    [ObservableProperty]
    bool _useFlashlight;
    partial void OnUseFlashlightChanged(bool value)
    {
        Preferences.Set(PreferenceKeys.UseFlashlight, value);
    }    

    [ObservableProperty]
    private bool _showPersonalizedAdButton;

    [ObservableProperty]
    bool _globalMute;
    partial void OnGlobalMuteChanged(bool value)
    {
        Preferences.Set(PreferenceKeys.MuteMetronome, value);
    }

    [ObservableProperty]
    List<InstrumentPicker> _rhythmInstruments;

    [ObservableProperty]
    List<InstrumentPicker> _beatInstruments;

    [ObservableProperty]
    private bool _showFlashlight;

    [ObservableProperty]
    private bool _showHaptic;
    partial void OnShowHapticChanged(bool value)
    {
        if (value)
        {
            var result = Permissions.CheckStatusAsync<Permissions.Vibrate>().Result;
            if (result == PermissionStatus.Granted)
                return;
            var keepGoing = _shellService.DisplayAlert(
                "Vibration Permission Needed",
                "In order to vibrate the device, you must give permission at the next prompt.",
                "OK",
                "Never mind").Result;

            if (keepGoing)
                result = Permissions.RequestAsync<Permissions.Vibrate>().Result;

            UseHaptic = result == PermissionStatus.Granted;
        }
        Preferences.Set(PreferenceKeys.UseHaptic, value);
    }

    [ObservableProperty]
    private bool _useHaptic;
    partial void OnUseHapticChanged(bool value)
    {
        Preferences.Set(PreferenceKeys.UseHaptic, value);
    }

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

    private IFlashlight _flashlight;
    private IVibration _vibration;
    private IShellService _shellService;
    private IDeviceInfo _deviceInfo;
    private IAppInfo _appInfo;

    public SettingsViewModel(IFlashlight flashlight, IVibration vibration, IShellService shellService, IDeviceInfo deviceInfo, IAppInfo appInfo)
    {
        _flashlight = flashlight;
        _vibration = vibration;
        _shellService = shellService;
        _deviceInfo = deviceInfo;
        _appInfo = appInfo;

        RhythmInstruments = InstrumentPicker.Instruments.Where(x => x.IsRhythm).ToList();
        BeatInstruments = InstrumentPicker.Instruments.Where(x => x.IsBeat).ToList();        
        ShowPersonalizedAdButton = _deviceInfo.Platform != DevicePlatform.Android;
        SendReminders = Preferences.Get(PreferenceKeys.PracticeReminders, false);
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

        UseFlashlight = Preferences.Get(PreferenceKeys.UseFlashlight, false);
        ShowHaptic = _vibration.IsSupported;
        UseHaptic = Preferences.Get(PreferenceKeys.UseHaptic, false);

        App.SetupAdmob();
    }

    [RelayCommand]
    void PersonalizedAdsChanged()
    {
        _appInfo.ShowSettingsUI();
    }
}

