using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    public InstantMetronomeViewModel(MetronomeClickerViewModel metronomeClickerViewModel)
    {
        _subdivision = SubdivisionEnum.Quarter;
        _metronomeClickerViewModel = metronomeClickerViewModel;

        BeatsPerMinute = 60;
        SelectedSubdivisionIndex = 0;

        SetupMetronome();
    }

    private bool _wasPlaying;

    public void Init()
    {
        _metronomeClickerViewModel.BeatType = ClickerBeatType.Instant;
        _metronomeClickerViewModel.IsLiveMode = false;
        _metronomeClickerViewModel.SetupMetronomeCommand.Execute(null);
    }

    [RelayCommand]
    private void PlayStopToggled()
    {
        _metronomeClickerViewModel.StartStopCommand.Execute(null);
        _wasPlaying = _metronomeClickerViewModel.IsPlaying;
    }

    [RelayCommand]
    private void SliderDragStarted()
    {
        _wasPlaying = _metronomeClickerViewModel.IsPlaying;
        _metronomeClickerViewModel.StopCommand.Execute(null);
    }

    [RelayCommand]
    private void SliderDragCompleted()
    {
        SetupMetronome();
        if (_wasPlaying)
            _metronomeClickerViewModel.StartStopCommand.Execute(null);
    }

    private void SetupMetronome()
    {
        _wasPlaying = _metronomeClickerViewModel.IsPlaying;
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
        if (_wasPlaying)
        {
            _metronomeClickerViewModel.StartStopCommand.Execute(null);
            _wasPlaying = _metronomeClickerViewModel.IsPlaying;
        }
    }
}
