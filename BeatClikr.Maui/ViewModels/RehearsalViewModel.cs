using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BeatClikr.Maui.Models;

namespace BeatClikr.Maui.ViewModels;

public partial class RehearsalViewModel : ObservableObject
{
    [ObservableProperty]
    private List<Song> _rehearsalSongPlayList = new List<Song>();

    [ObservableProperty]
    private bool _isPlaybackMode = true;
    partial void OnIsPlaybackModeChanged(bool value) => PlaybackCheckboxChanged();

    [ObservableProperty]
    private Song _selectedSong;

    private MetronomeClickerViewModel _metronomeClickerViewModel;
    private Services.Interfaces.IShellService _shellService;
    private Services.Interfaces.IDataService _persistence;

    public RehearsalViewModel(MetronomeClickerViewModel metronomeClickerViewModel,
        Services.Interfaces.IDataService persistence,
        Services.Interfaces.IShellService shellService)
    {
        _metronomeClickerViewModel = metronomeClickerViewModel;
        _persistence = persistence;
        _shellService = shellService;
    }

    public void Init()
    {
        RehearsalSongPlayList = _persistence.GetRehearsalSongs();
        _metronomeClickerViewModel.BeatType = ClickerBeatType.Rehearsal;
        _metronomeClickerViewModel.IsLiveMode = false;
        _metronomeClickerViewModel.SetSoundsCommand.Execute(null);
    }

    [RelayCommand]
    private void Cancel()
    {
        _shellService.GoToAsync("..");
    }

    [RelayCommand]
    private async void AddSongToPlaylist()
    {
        var addPage = ServiceHelper.GetService<Views.LibraryPage>() as Views.LibraryPage;
        addPage.Title = "Add to Rehearsal Playlist";
        addPage.Disappearing -= (s, e) => AddPageDisappearing(s as Views.LibraryPage);
        addPage.Disappearing += (s, e) => AddPageDisappearing(s as Views.LibraryPage);

        var addVm = ServiceHelper.GetService<LibraryViewModel>();
        addVm.AddToPlaylist = true;

        await _shellService.GoToAsync(RouteNames.LibraryRoute, true);
    }

    private void AddPageDisappearing(Views.LibraryPage page)
    {
        page.Title = "Library";
        page.Disappearing -= (s, e) => AddPageDisappearing(s as Views.LibraryPage);

        var addVm = ServiceHelper.GetService<LibraryViewModel>();
        addVm.AddToPlaylist = false;

        if (Song.Instance != null)
        {
            Song.Instance.RehearsalSequence = RehearsalSongPlayList.Count;
            _persistence.SaveSongToLibrary(Song.Instance);
        }
        Song.Instance = null;
        RehearsalSongPlayList = _persistence.GetRehearsalSongs();
    }

    [RelayCommand]
    private async void SongSelected()
    {
        if (SelectedSong == null)
            return;

        if (IsPlaybackMode)
        {
            _metronomeClickerViewModel.IsLiveMode = false;
            _metronomeClickerViewModel.StopCommand.Execute(true);
            _metronomeClickerViewModel.SetSongCommand.Execute(SelectedSong);
            _metronomeClickerViewModel.StartStopCommand.Execute(null);
        }
        else
        {
            string[] actionSheetOptions = new string[] { "Move to top", "Move to bottom" };
            string deleteOption = "Delete from Playlist";
            var result = await _shellService.DisplayActionSheet($"Editing {SelectedSong.Title}", "Cancel", deleteOption, actionSheetOptions);
            if (result == actionSheetOptions[0])
                MoveToTop();
            else if (result == actionSheetOptions[1])
                MoveToBottom();
            else if (result == deleteOption)
                RemoveFromPlaylist();
        }
        SelectedSong = null;
    }

    private void MoveToTop()
    {
        RehearsalSongPlayList.Remove(SelectedSong);
        RehearsalSongPlayList.Insert(0, SelectedSong);
        SetRehearsalSequence();
    }

    private void MoveToBottom()
    {
        RehearsalSongPlayList.Remove(SelectedSong);
        RehearsalSongPlayList.Add(SelectedSong);
        SetRehearsalSequence();
    }

    private void RemoveFromPlaylist()
    {
        SelectedSong.RehearsalSequence = null;
        _persistence.SaveSongToLibrary(SelectedSong);
        RehearsalSongPlayList.Remove(SelectedSong);
        SetRehearsalSequence();
    }

    private void SetRehearsalSequence()
    {
        if (RehearsalSongPlayList?.Count > 0)
        {
            for (int i = 0; i < RehearsalSongPlayList.Count; i++)
            {
                RehearsalSongPlayList[i].RehearsalSequence = i;
            }
            _persistence.SaveSongListToLibrary(RehearsalSongPlayList.ToList());
        }
        RehearsalSongPlayList = _persistence.GetRehearsalSongs();
    }

    [RelayCommand]
    private void Stop()
    {
        _metronomeClickerViewModel.StopCommand.Execute(null);
    }

    [RelayCommand]
    private void PlaybackCheckboxChanged()
    {
        if (_metronomeClickerViewModel.IsPlaying)
            _metronomeClickerViewModel.StopCommand.Execute(null);
        SelectedSong = null;
    }
}

