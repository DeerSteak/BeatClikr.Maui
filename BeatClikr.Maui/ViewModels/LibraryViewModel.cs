using BeatClikr.Maui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels;

public partial class LibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Library";

    [ObservableProperty]
    private Models.Song _selectedSong = null;

    [ObservableProperty]
    private bool _addToPlaylist;

    [ObservableProperty]
    private List<Models.Song> _filteredSongs = new List<Models.Song>();

    [ObservableProperty]
    private ImageSource _muteButtonImageSource;

    [ObservableProperty]
    private string _filter = string.Empty;
    partial void OnFilterChanged(string value)
    {
        var songs = _dataService.GetLibrarySongs(value);
        FilteredSongs = songs;
    }
    [ObservableProperty]
    private bool _isPlaybackMode = true;

    private MetronomeClickerViewModel _metronomeClickerViewModel;
    private Services.Interfaces.IShellService _shellService;
    private Services.Interfaces.IDataService _dataService;

		public LibraryViewModel(MetronomeClickerViewModel metronomeClickerViewModel,
			Services.Interfaces.IShellService shellService,
			Services.Interfaces.IDataService dataService)
		{
            _dataService = dataService;
            _shellService = shellService;
			_metronomeClickerViewModel = metronomeClickerViewModel;
		}

		public void Init()
		{
			Filter = string.Empty;
			OnFilterChanged(Filter);
            _metronomeClickerViewModel.BeatType = ClickerBeatType.Instant;
            _metronomeClickerViewModel.IsLiveMode = false;
            _metronomeClickerViewModel.SetSoundsCommand.Execute(null);
        }

    [RelayCommand]
    private void AddItem()
    {
        GoToSongDetails();
    }

    private void GoToSongDetails()
    {
        var addPage = ServiceHelper.GetService<Views.SongDetailsPage>();
        addPage.Disappearing -= (s, e) => OnFilterChanged(Filter);
        addPage.Disappearing += (s, e) => OnFilterChanged(Filter);

        var addVm = ServiceHelper.GetService<ViewModels.SongDetailsViewModel>();
        addVm.SongId = SelectedSong?.Id ?? null;

        _shellService.GoToAsync(RouteNames.SongDetailsRoute);
    }

    [RelayCommand]
    private void SelectionChanged()
    {
        if (SelectedSong == null)
            return;
        if (IsPlaybackMode && !AddToPlaylist)
        {
            _metronomeClickerViewModel.StopCommand.Execute(null);
            _metronomeClickerViewModel.SetSongCommand.Execute(SelectedSong);
            _metronomeClickerViewModel.StartStopCommand.Execute(null);
        }
        else if (AddToPlaylist)
        {
            Song.Instance = SelectedSong;
            _shellService.PopAsync();
        }
        else
        {
            GoToSongDetails();
        }
        SelectedSong = null;
    }

    [RelayCommand]
    private void Stop()
    {
        _metronomeClickerViewModel.StopCommand.Execute(null);
    }

    [RelayCommand]
    private void MuteToggle()
    {
        _metronomeClickerViewModel.MuteOverride = !_metronomeClickerViewModel.MuteOverride;
        Preferences.Set(PreferenceKeys.MuteMetronome, _metronomeClickerViewModel.MuteOverride);
    }

    private void FlashlightToggle()
    {

    }
}

