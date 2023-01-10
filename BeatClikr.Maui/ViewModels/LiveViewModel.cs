using System;
using CommunityToolkit.Mvvm.ComponentModel;
using BeatClikr.Maui.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BeatClikr.Maui.ViewModels
{
	public partial class LiveViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableCollection<Song> _liveSongPlaylist;

		[ObservableProperty]
		private bool _isPlaybackMode = true;

		[ObservableProperty]
		private string _adsId;

		[ObservableProperty]
		private Song _selectedSong;

		private CustomControls.MetronomeClickerViewModel _metronomeClickerViewModel;
		private Services.Interfaces.IShellService _shellService;
		private Services.Interfaces.IPersistence _persistence;

		public LiveViewModel(CustomControls.MetronomeClickerViewModel metronomeClickerViewModel,
			Services.Interfaces.IPersistence persistence,
			Services.Interfaces.IShellService shellService)
		{
			_metronomeClickerViewModel = metronomeClickerViewModel;
			_persistence = persistence;
			_shellService = shellService;
			AdsId = DeviceInfo.Platform == DevicePlatform.iOS
				? "ca-app-pub-8377432895177958/9628716771"
				: "ca-app-pub-8377432895177958/5497900071";
		}

		public void InitSongs()
		{
            var songList = _persistence.GetLiveSongs().Result;
            LiveSongPlaylist = new ObservableCollection<Song>();
			foreach (var song in songList)
				LiveSongPlaylist.Add(song);
        }

        [RelayCommand]
		private async void AddSongToPlaylist()
		{
			var addPage = ServiceHelper.GetService<Views.LibraryPage>() as Views.LibraryPage;
			addPage.Title = "Add to Live Playlist";
			addPage.Disappearing += (s, e) => AddPageDisappearing(s as Views.LibraryPage);
			Shell.SetPresentationMode(addPage, PresentationMode.ModalAnimated);
            addPage.ToolbarItems.Add(new ToolbarItem("CANCEL", "cancel", async () => { await _shellService.PopModalAsync(); }));
            await _shellService.PushModalAsync(addPage);
		}

		private void AddPageDisappearing(Views.LibraryPage page)
		{
			if (Song.Instance != null)
				LiveSongPlaylist.Add(Song.Instance);
			Shell.SetPresentationMode(page, PresentationMode.Animated);
			
			Song.Instance = null;
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
			LiveSongPlaylist.Remove(SelectedSong);
			LiveSongPlaylist.Insert(0, SelectedSong);
            await SetLiveSequence();
        }

		private async Task MoveToBottom()
		{
			LiveSongPlaylist.Remove(SelectedSong);
			LiveSongPlaylist.Add(SelectedSong);
            await SetLiveSequence();
        }

		private async Task RemoveFromPlaylist()
		{
            // find the item being removed and null its playlist index
            SelectedSong.LiveSequence = null;
            await _persistence.SaveSongToLibrary(SelectedSong);

			// remove the item from the list
			LiveSongPlaylist.Remove(SelectedSong);

            //refresh the list. 
            await SetLiveSequence();
        }

		private async Task SetLiveSequence()
		{
            if (LiveSongPlaylist?.Count > 0)
            {
                for (int i = 0; i < LiveSongPlaylist.Count; i++)
                {
                    LiveSongPlaylist[i].LiveSequence = i;
                }
                await _persistence.SaveSongListToLibrary(LiveSongPlaylist.ToList());
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
}

