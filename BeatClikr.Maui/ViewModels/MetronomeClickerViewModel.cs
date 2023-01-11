using System.Timers;
using BeatClikr.Maui.Constants;
using BeatClikr.Maui.Enums;
using BeatClikr.Maui.Helpers;
using BeatClikr.Maui.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;

namespace BeatClikr.Maui.ViewModels
{
    public partial class MetronomeClickerViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isPlaying;

        [ObservableProperty]
        private bool isLiveMode;

        [ObservableProperty]
        private ImageSource beatBox;

        [ObservableProperty]
        private ClickerBeatType beatType;

        [ObservableProperty]
        private Song song;

        [ObservableProperty]
        private bool isSilent;

        private System.Timers.Timer timer;
        private IAudioPlayer playerBeat1, playerRhythm1;
        private int subdivisionNumber, beatsPlayed;
        private readonly ImageSource bulb_dim, bulb_lit;
        private bool playSubdivisions;
        private readonly bool oniOS;
        private readonly IAudioManager audioManager;

        public MetronomeClickerViewModel(IAudioManager audioManager)
        {
            this.audioManager = audioManager;
            SetSounds(FileNames.Set1);
            bulb_dim = ImageSource.FromFile("bulb_dim");
            bulb_lit = ImageSource.FromFile("bulb_lit");
            BeatBox = bulb_dim;
            song = new Song();
            beatsPlayed = 0;
            oniOS = DeviceInfo.Platform == DevicePlatform.iOS;
        }

        [RelayCommand]
        private void SetSounds(string set)
        {
            string rhythm = string.Empty;
            string beat = string.Empty;

            switch (BeatType)
            {
                case ClickerBeatType.Live:
                    rhythm = Preferences.Get(PreferenceKeys.LiveRhythm, FileNames.HatClosed);
                    beat = Preferences.Get(PreferenceKeys.LiveBeat, FileNames.Kick);
                    break;
                case ClickerBeatType.Instant:
                    rhythm = Preferences.Get(PreferenceKeys.LiveRhythm, FileNames.HatClosed);
                    beat = Preferences.Get(PreferenceKeys.LiveBeat, FileNames.Kick);
                    break;
                case ClickerBeatType.Rehearsal:
                    rhythm = Preferences.Get(PreferenceKeys.LiveRhythm, FileNames.HatClosed);
                    beat = Preferences.Get(PreferenceKeys.LiveBeat, FileNames.Kick);
                    break;
                default:
                    rhythm = FileNames.HatClosed;
                    beat = FileNames.Kick;
                    break;
            }

            playerRhythm1 = audioManager.CreatePlayer(PlaybackUtilities.GetStreamFromFile(rhythm, set).Result);
            playerBeat1 = audioManager.CreatePlayer(PlaybackUtilities.GetStreamFromFile(beat, set).Result);
        }

        [RelayCommand]
        private void SetSong(Song song)
        {
            var wasPlaying = IsPlaying;
            if (IsPlaying)
                StopSongMetronome();
            this.song = song;
            if (wasPlaying)
                StartStop();
        }

        [RelayCommand]
        private void StartStop()
        {
            IsPlaying = !IsPlaying;
            if (IsPlaying)
                PlaySongMetronome();
            else
                StopSongMetronome();
        }

        [RelayCommand]
        private void Stop()
        {
            if (IsPlaying)
                StopSongMetronome();
        }

        private void PlaySongMetronome()
        {
            isSilent = false;
            float timerInterval = PlaybackUtilities.GetTimerInterval(song.Subdivision, song.BeatsPerMinute);
            playSubdivisions = song.Subdivision != SubdivisionEnum.Quarter;
            subdivisionNumber = 0;
            timer = new System.Timers.Timer(timerInterval) { AutoReset = true };
            timer.Elapsed += OnTimerElapsed;
            timer.Enabled = true;
        }

        private void StopSongMetronome()
        {
            timer.Enabled = false;
            BeatBox = bulb_dim;
            IsPlaying = false;
            IsSilent = false;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {

            if (IsLiveMode)
            {
                beatsPlayed++;
                if (beatsPlayed > song.BeatsPerMeasure)
                    isSilent = true;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (subdivisionNumber == 0)
                    BeatBox = bulb_lit;
                else
                    BeatBox = bulb_dim;
            });

            if (!isSilent)
            {
                if (subdivisionNumber == 0)
                {
                    MainThread.BeginInvokeOnMainThread(() => BeatBox = bulb_lit);
                    PlayBeat();
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() => BeatBox = bulb_dim);
                    if (playSubdivisions)
                        PlayRhythm();
                }
            }

            subdivisionNumber++;
            if (subdivisionNumber >= PlaybackUtilities.GetSubdivisionsPerBeat(song.Subdivision))
                subdivisionNumber = 0;
        }

        private void PlayRhythm()
        {
            if (oniOS)
            {
                playerRhythm1.Stop();
            }
            playerRhythm1.Seek(0);
            playerRhythm1.Play();
        }

        private void PlayBeat()
        {
            if (oniOS)
            {
                playerBeat1.Stop();
            }
            playerBeat1.Seek(0);
            playerBeat1.Play();
        }
    }
}

