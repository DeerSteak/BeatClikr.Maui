using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels;

public partial class MetronomeViewModel : ObservableObject
{
    private SubdivisionEnum _subdivision;
    private int _numBeats;
    private int _tempo;
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
    private string _beatsPerMeasure;
    partial void OnBeatsPerMeasureChanged(string value)
    {
        if (int.TryParse(_beatsPerMeasure, out _tempo))
            SetupMetronome();
    }

    [ObservableProperty]
    private string _beatsPerMinute;
    partial void OnBeatsPerMinuteChanged(string value)
    {
        if (int.TryParse(_beatsPerMinute, out _numBeats))
            SetupMetronome();
    }

    [ObservableProperty]
    private string[] _subdivisions = new string[] { "Quarter", "Eighth", "Eighth Triplet", "Sixteenth" };

    [ObservableProperty]
    private string _adsId;

    public MetronomeViewModel(MetronomeClickerViewModel metronomeClickerViewModel)
    {
        _metronomeClickerViewModel = metronomeClickerViewModel;
        _numBeats = 4;
        _tempo = 60;
        _subdivision = SubdivisionEnum.Eighth;

        BeatsPerMeasure = "4";
        BeatsPerMinute = "60";
        AdsId = DeviceInfo.Platform == DevicePlatform.iOS
            ? "ca-app-pub-8377432895177958/7490720167"
            : "ca-app-pub-8377432895177958/9298625858";
        SelectedSubdivisionIndex = 1;

        SetupMetronome();
    }

    [RelayCommand]
    private void PlayStopToggled()
    {
        _metronomeClickerViewModel.StartStopCommand.Execute(null);
    }

    private void SetupMetronome()
    {
        var wasPlaying = _metronomeClickerViewModel.IsPlaying;
        if (wasPlaying)
            _metronomeClickerViewModel.StopCommand.Execute(null);
        var song = new Models.Song()
        {
            Title = "Instant metronome",
            Artist = "BeatClikr",
            BeatsPerMeasure = _tempo,
            BeatsPerMinute = _numBeats,
            Subdivision = _subdivision
        };
        _metronomeClickerViewModel.Song = song;
        if (wasPlaying)
            _metronomeClickerViewModel.StartStopCommand.Execute(null);
    }
}
