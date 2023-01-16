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
			Id = value.Id;
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
                case 1:
                    Subdivision = SubdivisionEnum.Eighth;
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
        }

        [ObservableProperty]
        private string[] _subdivisions = new string[] { "Quarter", "Eighth", "Eighth Triplet", "Sixteenth" };

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
				Song = new Song()
				{
					BeatsPerMeasure = 4,
					BeatsPerMinute = 60,
					Subdivision = SubdivisionEnum.Eighth
				};
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
			var result = 0;
			if (_recordChanged)
				result = await _persistence.SaveSongToLibrary(Song);
			try
			{
                await _shellService.PopAsync();

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
}

