using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels;

public partial class MetronomeViewModel : ObservableObject
{
    [ObservableProperty]
    private string _beatsPerMeasure;

    [ObservableProperty]
    private string[] _subdivisions = new string[] { "Quarter", "Eighth", "Eighth Triplet", "Sixteenth" };

    [ObservableProperty]
    private string _beatsPerMinute;

    [ObservableProperty]
    private string _adsId;

    private CustomControls.MetronomeClickerViewModel _metronomeClickerViewModel;

    public MetronomeViewModel(CustomControls.MetronomeClickerViewModel metronomeClickerViewModel)
    {
        _metronomeClickerViewModel = metronomeClickerViewModel;
        BeatsPerMeasure = "4";
        BeatsPerMinute = "60";
        AdsId = DeviceInfo.Platform == DevicePlatform.iOS
            ? "ca-app-pub-8377432895177958/7490720167"
            : "ca-app-pub-8377432895177958/9298625858";
    }

    [RelayCommand]
    private void BeatsPerMinuteChanged()
    {

    }

    [RelayCommand]
    private void PlayStopToggled()
    {

    }
}

