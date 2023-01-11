using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels;

public partial class RehearsalViewModel : ObservableObject
{
    private MetronomeClickerViewModel _metronomeClickerViewModel;
    private Services.Interfaces.IShellService _shellService;
    private Services.Interfaces.IDataService _persistence;

    [ObservableProperty]
    private ObservableCollection<Models.Song> _rehearsalSongPlayList;

    [ObservableProperty]
    private string _adsId;

    [ObservableProperty]
    private Models.Song _selectedSong;

    [ObservableProperty]
    private bool _isPlaybackMode = true;
    partial void OnIsPlaybackModeChanged(bool value) => PlaybackCheckboxChanged();

    public RehearsalViewModel(MetronomeClickerViewModel metronomeClickerViewModel,
        Services.Interfaces.IShellService shellService,
        Services.Interfaces.IDataService persistence)
    {
        _metronomeClickerViewModel = metronomeClickerViewModel;
        _metronomeClickerViewModel.IsLiveMode = false;
        _shellService = shellService;
        _persistence = persistence;

        IsPlaybackMode = true;
        AdsId = DeviceInfo.Platform == DevicePlatform.iOS
            ? "ca-app-pub-8377432895177958/8507206795"
            : "ca-app-pub-8377432895177958/8149195717";
    }

    public void InitSongs()
    {
        var songList = _persistence.GetRehearsalSongs().Result;
        RehearsalSongPlayList = new ObservableCollection<Models.Song>();
        foreach (var song in songList)
            RehearsalSongPlayList.Add(song);
    }

    [RelayCommand]
    private async void AddSongToPlaylist()
    {
        var addPage = ServiceHelper.GetService<Views.LibraryPage>() as Views.LibraryPage;
        addPage.Title = "Add to Rehearsal Playlist";
        addPage.Disappearing += (s, e) => AddPageDisappearing(s as Views.LibraryPage);
        Shell.SetPresentationMode(addPage, PresentationMode.ModalAnimated);
        addPage.ToolbarItems.Add(new ToolbarItem("CANCEL", "cancel", async () => { await _shellService.PopModalAsync(); }));
        await _shellService.PushModalAsync(addPage);
    }

    private void AddPageDisappearing(Views.LibraryPage page)
    {
        if (Models.Song.Instance != null)
            RehearsalSongPlayList.Add(Models.Song.Instance);
        Shell.SetPresentationMode(page, PresentationMode.Animated);

        Models.Song.Instance = null;
    }

    [RelayCommand]
    private async void SongSelected()
    {
        if (SelectedSong == null)
            return;

        if (IsPlaybackMode)
        {
            _metronomeClickerViewModel.IsLiveMode = true;
            _metronomeClickerViewModel.SetSongCommand.Execute(SelectedSong);
            _metronomeClickerViewModel.StartStopCommand.Execute(null);
        }
        else
        {
            string[] actionSheetOptions = new string[] { "Move to top", "Move to bottom" };
            string deleteOption = "Delete from Playlist";
            var result = await _shellService.DisplayActionSheet($"Editing {SelectedSong.Title}", "Cancel", deleteOption, actionSheetOptions);
            if (result == actionSheetOptions[0])
                await MoveToTop().ConfigureAwait(true);
            else if (result == actionSheetOptions[1])
                await MoveToBottom().ConfigureAwait(true);
            else if (result == deleteOption)
                await RemoveFromPlaylist().ConfigureAwait(true);

            SelectedSong = null;
        }
    }

    private async Task MoveToTop()
    {
        RehearsalSongPlayList.Remove(SelectedSong);
        RehearsalSongPlayList.Insert(0, SelectedSong);
        await SetRehearsalSequence();
    }

    private async Task MoveToBottom()
    {
        RehearsalSongPlayList.Remove(SelectedSong);
        RehearsalSongPlayList.Add(SelectedSong);
        await SetRehearsalSequence();
    }

    private async Task RemoveFromPlaylist()
    {
        SelectedSong.RehearsalSequence = null;
        await _persistence.SaveSongToLibrary(SelectedSong);
        RehearsalSongPlayList.Remove(SelectedSong);
        await SetRehearsalSequence();
    }

    private async Task SetRehearsalSequence()
    {
        if (RehearsalSongPlayList?.Count > 0)
        {
            for (int i = 0; i < RehearsalSongPlayList.Count; i++)
            {
                RehearsalSongPlayList[i].RehearsalSequence = i;
            }
            await _persistence.SaveSongListToLibrary(RehearsalSongPlayList.ToList());
            //var list = await _persistence.GetLiveSongs();
        }
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

