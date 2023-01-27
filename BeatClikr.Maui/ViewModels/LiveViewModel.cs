using System;
using CommunityToolkit.Mvvm.ComponentModel;
using BeatClikr.Maui.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BeatClikr.Maui.ViewModels;

public partial class LiveViewModel : ObservableObject
{
    [ObservableProperty]
    private List<Song> _liveSongPlaylist = new List<Song>();

    [ObservableProperty]
    private bool _isPlaybackMode = true;
    partial void OnIsPlaybackModeChanged(bool value) => PlaybackCheckboxChanged();

    [ObservableProperty]
    private Song _selectedSong;

    private MetronomeClickerViewModel _metronomeClickerViewModel;
    private Services.Interfaces.IShellService _shellService;
    private Services.Interfaces.IDataService _persistence;

    public LiveViewModel(MetronomeClickerViewModel metronomeClickerViewModel,
        Services.Interfaces.IDataService persistence,
        Services.Interfaces.IShellService shellService)
    {
        _metronomeClickerViewModel = metronomeClickerViewModel;
        _persistence = persistence;
        _shellService = shellService;
    }

    public void Init()
    {
        LiveSongPlaylist = _persistence.GetLiveSongs();
        _metronomeClickerViewModel.BeatType = ClickerBeatType.Live;
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
        addPage.Title = "Add to Live Playlist";
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
            Song.Instance.LiveSequence = LiveSongPlaylist.Count;
            _persistence.SaveSongToLibrary(Song.Instance);
        }
        Song.Instance = null;
        LiveSongPlaylist = _persistence.GetLiveSongs();
    }

    [RelayCommand]
    private async void SongSelected()
    {
        if (SelectedSong == null)
            return;

        if (IsPlaybackMode)
        {
            _metronomeClickerViewModel.IsLiveMode = true;
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
        LiveSongPlaylist.Remove(SelectedSong);
        LiveSongPlaylist.Insert(0, SelectedSong);
        SetLiveSequence();
    }

    private void MoveToBottom()
    {
        LiveSongPlaylist.Remove(SelectedSong);
        LiveSongPlaylist.Add(SelectedSong);
        SetLiveSequence();
    }

    private void RemoveFromPlaylist()
    {
        SelectedSong.LiveSequence = null;
        _persistence.SaveSongToLibrary(SelectedSong);
        LiveSongPlaylist.Remove(SelectedSong);
        SetLiveSequence();
    }

    private void SetLiveSequence()
    {
        if (LiveSongPlaylist?.Count > 0)
        {
            for (int i = 0; i < LiveSongPlaylist.Count; i++)
            {
                LiveSongPlaylist[i].LiveSequence = i;
            }
            _persistence.SaveSongListToLibrary(LiveSongPlaylist.ToList());
        }
        LiveSongPlaylist = _persistence.GetLiveSongs();
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

