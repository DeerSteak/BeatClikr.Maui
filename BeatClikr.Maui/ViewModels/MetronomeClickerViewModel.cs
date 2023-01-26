using System.Timers;
using BeatClikr.Maui.Constants;
using BeatClikr.Maui.Enums;
using BeatClikr.Maui.Helpers;
using BeatClikr.Maui.Models;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;

namespace BeatClikr.Maui.ViewModels
{
    public partial class MetronomeClickerViewModel : ObservableObject
    {
        private System.Timers.Timer _timer;
        private IAudioPlayer _playerBeat;
        private IAudioPlayer _playerRhythm;
        private int _subdivisionNumber;
        private int _beatsPlayed = 0;
        private readonly ImageSource _bulbDim;
        private readonly ImageSource _bulbLit;
        private bool _playSubdivisions;
        private readonly bool _onApple;
        private readonly IAudioManager _audioManager;
        private bool _flashlightEnabled;
        //private IconTintColorBehavior _behavior = new IconTintColorBehavior();

        [ObservableProperty]
        private bool _isPlaying;

        [ObservableProperty]
        private bool _isLiveMode;

        [ObservableProperty]
        private ImageSource _beatBox;

        [ObservableProperty]
        private bool _muteOverride;

        [ObservableProperty]
        private ClickerBeatType _beatType;
        partial void OnBeatTypeChanged(ClickerBeatType value)
        {
            SetSounds(FileNames.Set1);
        }

        [ObservableProperty]
        private Song _song;

        [ObservableProperty]
        private bool _isSilent;

        public MetronomeClickerViewModel(IAudioManager audioManager, IAppInfo appInfo)
        {
            _audioManager = audioManager;
            
            _onApple = DeviceInfo.Platform == DevicePlatform.iOS
                || DeviceInfo.Platform == DevicePlatform.MacCatalyst;

            var currentTheme = appInfo.RequestedTheme;            
            //_behavior.TintColor = appInfo.RequestedTheme == AppTheme.Light
            //    ? Color.FromArgb("#212121")
            //    : Color.FromArgb("#FAFAFA");

            _bulbDim = new FontImageSource()
            {
                FontFamily = "FARegular",
                Glyph = Constants.IconFont.Lightbulb,
                Color = appInfo.RequestedTheme == AppTheme.Dark ? Color.FromArgb("#FAFAFA") : Color.FromArgb("#212121"),
                Size = 90
            };

            _bulbLit = new FontImageSource()
            {
                FontFamily = "FARegular",
                Glyph = Constants.IconFont.LightbulbOn,
                Color = appInfo.RequestedTheme == AppTheme.Dark ? Color.FromArgb("#FAFAFA") : Color.FromArgb("#212121"),
                Size = 90
            };

            _flashlightEnabled = true;

            BeatBox = _bulbDim;
            Song = new Song();
        }

        [RelayCommand]
        private void SetSong(Song song)
        {
            var wasPlaying = IsPlaying;
            if (IsPlaying)
                StopSongMetronome();
            this.Song = song;
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
            _beatsPlayed = 0;
            IsSilent = !MuteOverride;
            float timerInterval = PlaybackUtilities.GetTimerInterval(Song.Subdivision, Song.BeatsPerMinute);
            _playSubdivisions = Song.Subdivision != SubdivisionEnum.Quarter;
            _subdivisionNumber = 0;
            _timer = new System.Timers.Timer(timerInterval) { AutoReset = true };
            _timer.Elapsed += OnTimerElapsed;
            _timer.Enabled = true;
        }

        private void StopSongMetronome()
        {
            _timer.Enabled = false;
            BeatBox = _bulbDim;
            //(BeatBox.Parent as Image).Behaviors.Add(_behavior);
            IsPlaying = false;
            IsSilent = false;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {

            if (IsLiveMode && !IsSilent && _subdivisionNumber == 0)
            {
                if (_beatsPlayed >= Song.BeatsPerMeasure)
                    IsSilent = true;
            }

            if (_subdivisionNumber == 0)
                BeatBox = _bulbLit;
            else
                BeatBox = _bulbDim;

            if (!IsSilent)
            {
                if (_subdivisionNumber == 0)
                {
                    _beatsPlayed++;
                    Task.Run(() => Flashlight.Default.TurnOnAsync().Start());
                    PlayBeat();
                }
                else
                {
                    Task.Run(() => Flashlight.Default.TurnOffAsync().Start());
                    if (_playSubdivisions)
                        PlayRhythm();
                }
            }

            _subdivisionNumber++;
            if (_subdivisionNumber >= PlaybackUtilities.GetSubdivisionsPerBeat(Song.Subdivision))
                _subdivisionNumber = 0;
        }

        private void PlayRhythm()
        {
            if (_onApple)
            {
                _playerRhythm.Stop();
            }
            _playerRhythm.Seek(0);
            _playerRhythm.Play();
        }

        private void PlayBeat()
        {
            if (_onApple)
            {
                _playerBeat.Stop();
            }
            _playerBeat.Play();
        }

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

            _playerRhythm = _audioManager.CreatePlayer(PlaybackUtilities.GetStreamFromFile(rhythm, set).Result);
            _playerBeat = _audioManager.CreatePlayer(PlaybackUtilities.GetStreamFromFile(beat, set).Result);
        }
    }
}

