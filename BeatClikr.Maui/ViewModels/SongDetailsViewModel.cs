using System;
using BeatClikr.Maui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels
{
	[QueryProperty(nameof(Id), nameof(Id))]
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
			Id = value.ID;
			RehearsalSequence = value.RehearsalSequence;
			LiveSequence = value.LiveSequence;
			_recordChanged = false;
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
            SyncSongAndMetronome();
        }

        [ObservableProperty]
		private int? _id;
        partial void OnIdChanged(int? value)
        {
            if (value != null)
			{
				Song = _persistence.GetById(value.GetValueOrDefault()).Result;
			}
			else
			{
				Song = new Song();
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
		}

		[RelayCommand]
		private async void Cancel()
		{
			var stayPut = false;
			if (_recordChanged)
			    stayPut = await _shellService.DisplayAlert("Cancel?", "Any unsaved changes will be lost", "Yes", "No");
			if (!stayPut)
				await _shellService.GoToAsync("..");
		}

		[RelayCommand]
		private async void Save()
		{
			if (_recordChanged)
				await _persistence.SaveSongToLibrary(Song);
			await _shellService.GoToAsync("..");
		}

		[RelayCommand]
		private void PlayPause()
		{
			_metronomeClickerViewModel.StartStopCommand.Execute(null);
		}
	}
}

