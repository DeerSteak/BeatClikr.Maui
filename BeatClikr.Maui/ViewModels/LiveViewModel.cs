﻿using System;
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

		public void InitSongs()
		{
            var songList = _persistence.GetLiveSongs().Result;
            LiveSongPlaylist = new ObservableCollection<Song>();
			foreach (var song in songList)
				LiveSongPlaylist.Add(song);
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
			addPage.Disappearing += (s, e) => AddPageDisappearing(s as Views.LibraryPage);
			Shell.SetPresentationMode(addPage, PresentationMode.ModalAnimated);
			var cancelButton = new ToolbarItem()
			{
				Text = "CANCEL",
				IconImageSource = new FontImageSource()
                {
                    Glyph = Constants.IconFont.Ban,
                    FontFamily = "FARegular"
                },
			    Command = CancelCommand
			};
            addPage.ToolbarItems.Add(cancelButton);
            await _shellService.PushModalAsync(addPage);
		}

		private void AddPageDisappearing(Views.LibraryPage page)
		{
			if (Song.Instance != null)
				LiveSongPlaylist.Add(Song.Instance);
			Shell.SetPresentationMode(page, PresentationMode.Animated);
			page.ToolbarItems.Clear();
			page.Title = "Library";
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
            SelectedSong.LiveSequence = null;
            await _persistence.SaveSongToLibrary(SelectedSong);
			LiveSongPlaylist.Remove(SelectedSong);
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

