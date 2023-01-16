using System;
using System.Collections.ObjectModel;
using BeatClikr.Maui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels
{
	public partial class LibraryViewModel : ObservableObject
	{
		[ObservableProperty]
		private string _title = "Library";

		[ObservableProperty]
		private Models.Song _selectedSong = null;

        [ObservableProperty]
		private List<Models.Song> _filteredSongs = new List<Models.Song>();

		[ObservableProperty]
		private string _filter = string.Empty;
		partial void OnFilterChanged(string value)
		{
			var songs = _dataService.GetLibrarySongs(value).Result;
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
			_metronomeClickerViewModel.BeatType = ClickerBeatType.Instant;
			_metronomeClickerViewModel.IsLiveMode = false;
		}

		public void Init()
		{
			Filter = string.Empty;
			OnFilterChanged(Filter);
		}

		[RelayCommand]
		private void AddItem()
		{
			var addPage = ServiceHelper.GetService<Views.SongDetailsPage>();
			addPage.Disappearing += (s, e) => OnFilterChanged(Filter);
			_shellService.PushAsync(addPage);
		}

		private void ItemAdded(object s, EventArgs e)
		{
			OnFilterChanged(Filter);
		}

		[RelayCommand]
		private void SelectionChanged()
		{
			if (SelectedSong == null)
				return;
			if (IsPlaybackMode)
			{
				_metronomeClickerViewModel.StopCommand.Execute(null);
				_metronomeClickerViewModel.SetSongCommand.Execute(SelectedSong);
				_metronomeClickerViewModel.StartStopCommand.Execute(null);
			}
			else
			{
				_shellService.GoToAsync($"{RouteNames.SongDetailsRoute}?Id={SelectedSong.Id}");
			}
			SelectedSong = null;
		}

		[RelayCommand]
		private void Stop()
		{
			if (IsPlaybackMode)
				_metronomeClickerViewModel.StopCommand.Execute(null);
		}
	}
}

