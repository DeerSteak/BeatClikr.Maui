using BeatClikr.Maui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels;

public partial class SongDetailsViewModel : ObservableObject
{
    private Services.Interfaces.IDataService _persistence;
    private Services.Interfaces.IShellService _shellService;
    private MetronomeClickerViewModel _metronomeClickerViewModel;
    private bool _recordChanged = false;

    [ObservableProperty]
    private Models.Song _song = new Song();
    partial void OnSongChanged(Song value)
    {
        Title = value.Title;
        Artist = value.Artist;
        BeatsPerMeasure = value.BeatsPerMeasure;
        BeatsPerMinute = value.BeatsPerMinute;
        Subdivision = value.Subdivision;
        RehearsalSequence = value.RehearsalSequence;
        LiveSequence = value.LiveSequence;
        _recordChanged = false;
        _metronomeClickerViewModel.BeatType = ClickerBeatType.Instant;
        _metronomeClickerViewModel.SetSongCommand.Execute(value);
    }

    [ObservableProperty]
    private string _title;
    partial void OnTitleChanged(string value)
    {
        Song.Title = value;
        _recordChanged = true;
    }

    [ObservableProperty]
    private string _artist;
    partial void OnArtistChanged(string value)
    {
        Song.Artist = value;
        _recordChanged = true;
    }

    [ObservableProperty]
    private int _beatsPerMeasure;
    partial void OnBeatsPerMeasureChanged(int value)
    {
        Song.BeatsPerMeasure = value;
        SyncSongAndMetronome();
    }

    private void SyncSongAndMetronome()
    {
        _recordChanged = true;
        _metronomeClickerViewModel.SetSongCommand.Execute(Song);
    }

    [ObservableProperty]
    private int _beatsPerMinute;
    partial void OnBeatsPerMinuteChanged(int value)
    {
        Song.BeatsPerMinute = value;
        SyncSongAndMetronome();
    }

    [ObservableProperty]
    private SubdivisionEnum _subdivision;
    partial void OnSubdivisionChanged(SubdivisionEnum value)
    {
        Song.Subdivision = value;
        switch (value)
        {
            case SubdivisionEnum.Eighth:
                SelectedSubdivisionIndex = 1;
                break;
            case SubdivisionEnum.TripletEighth:
                SelectedSubdivisionIndex = 2;
                break;
            case SubdivisionEnum.Sixteenth:
                SelectedSubdivisionIndex = 3;
                break;
            default:
                SelectedSubdivisionIndex = 0;
                break;
        }
        SyncSongAndMetronome();
    }

    [ObservableProperty]
    private int _selectedSubdivisionIndex;
    partial void OnSelectedSubdivisionIndexChanged(int value)
    {
        switch (value)
        {
            case 0:
                Subdivision = SubdivisionEnum.Quarter;
                break;
            case 2:
                Subdivision = SubdivisionEnum.TripletEighth;
                break;
            case 3:
                Subdivision = SubdivisionEnum.Sixteenth;
                break;
            default:
                Subdivision = SubdivisionEnum.Eighth;
                break;
        }
        SyncSongAndMetronome();
    }

    [ObservableProperty]
    private string[] _subdivisions = new string[] { "Quarter", "Eighth", "Eighth Triplet", "Sixteenth" };

    [ObservableProperty]
    private int? _songId;
    partial void OnSongIdChanged(int? value)
    {
        if (value != null)
        {
            Song = _persistence.GetById(value.GetValueOrDefault());
            BeatsPerMinute = Song.BeatsPerMinute;
            BeatsPerMeasure = Song.BeatsPerMeasure;
            Subdivision = Song.Subdivision;
            switch (Subdivision)
            {
                case SubdivisionEnum.Quarter:
                    SelectedSubdivisionIndex = 0;
                    break;
                case SubdivisionEnum.TripletEighth:
                    SelectedSubdivisionIndex = 2;
                    break;
                case SubdivisionEnum.Sixteenth:
                    SelectedSubdivisionIndex = 3;
                    break;
                default: //Eighth or anything else I forgot to add
                    SelectedSubdivisionIndex = 1;
                    break;
            }
            Artist = Song.Artist;
            Title = Song.Title;
        }
        else
        {
            Song = new Song()
            {
                BeatsPerMeasure = 4,
                BeatsPerMinute = 60,
                Subdivision = SubdivisionEnum.Eighth
            };
            SelectedSubdivisionIndex = 2;
            BeatsPerMinute = Song.BeatsPerMinute;
            BeatsPerMeasure = Song.BeatsPerMeasure;
            Title = "";
            Artist = "";
        }
        _recordChanged = false;
        _metronomeClickerViewModel.SetSongCommand.Execute(Song);
    }

    [ObservableProperty]
    private int? _rehearsalSequence;
    partial void OnRehearsalSequenceChanged(int? value)
    {
        Song.RehearsalSequence = value;
        SyncSongAndMetronome();
    }

    [ObservableProperty]
    private int? _liveSequence;
    partial void OnLiveSequenceChanged(int? value)
    {
        Song.LiveSequence = value;
        SyncSongAndMetronome();
    }

    public SongDetailsViewModel(Services.Interfaces.IDataService persistence,
            Services.Interfaces.IShellService shellService,
            MetronomeClickerViewModel metronomeClickerViewModel)
    {
        _persistence = persistence;
        _shellService = shellService;
        _metronomeClickerViewModel = metronomeClickerViewModel;
        _metronomeClickerViewModel.IsLiveMode = false;
        _metronomeClickerViewModel.BeatType = ClickerBeatType.Instant;
        Song = new Song();
        OnSubdivisionChanged(Song.Subdivision);
        _recordChanged = false;
    }

    [RelayCommand]
    private async void Cancel()
    {
        var stayPut = false;
        if (_recordChanged)
            stayPut = await _shellService.DisplayAlert("Cancel?", "Any unsaved changes will be lost", "Yes", "No");
        if (!stayPut)
            await _shellService.GoToAsync("..", true);
    }

    [RelayCommand]
    private async void Save()
    {
        var result = 0;
        if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Artist) || BeatsPerMinute <= 0)
        {
            await _shellService.DisplayAlert("Incomplete Form", "Both the Artist and Title should be set, and the BPM must be greater than 0", "OK");
            return;
        }

        if (_recordChanged)
            result = _persistence.SaveSongToLibrary(Song);
        try
        {
            await _shellService.PopAsync();
            if (SongId != null)
                SongId = null;
            else
                Song = new Song();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    private void PlayPause()
    {
        _metronomeClickerViewModel.StartStopCommand.Execute(null);
    }
}

