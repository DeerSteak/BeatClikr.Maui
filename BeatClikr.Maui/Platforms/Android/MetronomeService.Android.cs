using Android.Content;
using Android.Media;
using Android.OS;
using BeatClikr.Maui.Services.Interfaces;
using Java.Util;
using Java.Util.Concurrent;

namespace BeatClikr.Maui.Platforms.Android;

public class MetronomeService : IMetronomeService
{
    private int _bpm;
    private int _subdivisions;
    private bool _playSubdivisions;
    private const int SAMPLE_RATE = 44100;
    private const int WAV_BUFFER_OFFSET = 44;
    private ScheduledThreadPoolExecutor _timer;
    private int _timerEventCounter;
    private int _beatsPlayed;
    private bool _liveModeStarted;

    private byte[] _beatBuffer;
    private byte[] _rhythmBuffer;
    private readonly static byte[] _silenceBuffer = SetSilenceBuffer();
    private readonly int _minBufferSize;

    private AudioTrack _audioTrack;
    double _subdivisionLengthInMilliseconds;
    private bool _useHaptic;
    private VibrationEffect _beatEffect;
    private VibrationEffect _rhythmEffect;
    private readonly bool _canVibrate;

    static Vibrator vibrator;

    static Vibrator Vibrator =>
        vibrator ??= GetVibrator();

    //different ways to do this for Android 30 and below, vs. Android 31 and above.
    private static Vibrator GetVibrator()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            var mgr = MainApplication.Context.GetSystemService(Context.VibratorManagerService) as VibratorManager;
            return mgr?.DefaultVibrator;
        }
        else
            return MainApplication.Context.GetSystemService(Context.VibratorService) as Vibrator;
    }

    public MetronomeService()
    {
        _minBufferSize = AudioTrack.GetMinBufferSize(SAMPLE_RATE, ChannelOut.Stereo, Encoding.Pcm16bit);
        _canVibrate = Vibrator == null;
        SetHaptic();
    }

    public void SetHaptic()
    {
        _useHaptic = Preferences.Get(PreferenceKeys.UseHaptic, false);
        if (_useHaptic)
        {
            _beatEffect = VibrationEffect.CreateOneShot(10, 255);
            _rhythmEffect = VibrationEffect.CreateOneShot(10, 128);
        }
    }

    public void Play()
    {
        _timerEventCounter = 0;
        _beatsPlayed = 0;
        _liveModeStarted = false;
        StartTimer();
    }

    public void SetBeat(string fileName, string set)
    {
        _beatBuffer = SetBuffer(fileName, set);
    }

    public void SetRhythm(string fileName, string set)
    {
        _rhythmBuffer = SetBuffer(fileName, set);
    }

    private static byte[] SetBuffer(string fileName, string set)
    {
        byte[] bytes;
        using System.IO.Stream fileStream = FileSystem.Current.OpenAppPackageFileAsync($"{set}/{fileName}.wav").Result;
        using (MemoryStream memoryStream = new())
        {
            fileStream.CopyTo(memoryStream);
            var buffer = memoryStream.GetBuffer();
            if (buffer.Length < _silenceBuffer.Length)
            {
                bytes = new byte[_silenceBuffer.Length - WAV_BUFFER_OFFSET - WAV_BUFFER_OFFSET];
                Buffer.BlockCopy(buffer, WAV_BUFFER_OFFSET, bytes, 0, buffer.Length - WAV_BUFFER_OFFSET);
                Buffer.BlockCopy(_silenceBuffer, WAV_BUFFER_OFFSET, bytes, buffer.Length - WAV_BUFFER_OFFSET, _silenceBuffer.Length - buffer.Length - 44);
            }
            else
            {
                bytes = new byte[buffer.Length - WAV_BUFFER_OFFSET];
                Buffer.BlockCopy(buffer, WAV_BUFFER_OFFSET, bytes, 0, buffer.Length - WAV_BUFFER_OFFSET);
            }
        }

        return bytes;
    }

    private static byte[] SetSilenceBuffer()
    {
        using System.IO.Stream fileStream = FileSystem.Current.OpenAppPackageFileAsync($"{FileNames.Set1}/{FileNames.Silence}.wav").Result;
        using MemoryStream memoryStream = new();
        fileStream.CopyTo(memoryStream);
        var buffer = memoryStream.GetBuffer();
        return buffer;
    }

    public void SetTempo(int bpm, int subdivisions)
    {
        _bpm = bpm switch
        {
            < 60 => 60,
            > 240 => 240,
            _ => bpm,
        };
        switch (subdivisions)
        {
            case <= 1:
                _subdivisions = 2;
                _playSubdivisions = false;
                break;
            case > 4:
                _subdivisions = 4;
                _playSubdivisions = true;
                break;
            default:
                _subdivisions = subdivisions;
                _playSubdivisions = true;
                break;
        }
        _subdivisionLengthInMilliseconds = (60.0 / (_bpm * _subdivisions)) * 1000;
    }

    private void StartTimer()
    {
        _audioTrack.SetVolume(0);
        var bytesWritten = _audioTrack.Write(_silenceBuffer, 0, _silenceBuffer.Length, WriteMode.NonBlocking);
        _audioTrack.Play();
        _audioTrack.SetVolume(.9f);

        var task = new MetronomeTimerTask
        {
            Action = HandleTimer
        };

        _timer = new ScheduledThreadPoolExecutor(_subdivisions);
        _timer.ScheduleAtFixedRate(task, 0, (long)_subdivisionLengthInMilliseconds, TimeUnit.Milliseconds);
    }

    public void SetupMetronome(string beatFileName, string rhythmFileName, string set)
    {
        SetBeat(beatFileName, set);
        SetRhythm(rhythmFileName, set);
        SetTempo(_bpm, _subdivisions);

        _subdivisionLengthInMilliseconds = 60000D / (double)(_bpm * _subdivisions);

        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            _audioTrack = new AudioTrack.Builder()
                .SetAudioAttributes(GetAudioAttributesBuilder().Build())
                .SetAudioFormat(new AudioFormat.Builder()
                    .SetChannelMask(ChannelOut.Stereo)
                    .SetEncoding(Encoding.Pcm16bit)
                    .SetSampleRate(SAMPLE_RATE)
                    .Build())
                .SetTransferMode(AudioTrackMode.Stream)
                .SetBufferSizeInBytes(_minBufferSize)
                .Build();
            _audioTrack.SetVolume(0);
        }
    }

    private static AudioAttributes.Builder GetAudioAttributesBuilder()
    {
        var audioAttributesBuilder = new AudioAttributes.Builder()
            .SetContentType(AudioContentType.Music)
            .SetFlags(AudioFlags.AudibilityEnforced)
            .SetUsage(AudioUsageKind.Media);

        if (OperatingSystem.IsAndroidVersionAtLeast(29))
            audioAttributesBuilder = audioAttributesBuilder
                .SetAllowedCapturePolicy(CapturePolicies.ByAll)
                .SetHapticChannelsMuted(false);

        if (OperatingSystem.IsAndroidVersionAtLeast(32))
            audioAttributesBuilder = audioAttributesBuilder
                .SetIsContentSpatialized(false)
                .SetSpatializationBehavior((int)AudioSpatializationBehavior.Never);

        return audioAttributesBuilder;
    }

    public void Stop()
    {
        _timer?.ShutdownNow();

        if (_audioTrack?.PlayState == PlayState.Playing)
        {
            _audioTrack.SetVolume(0);
            _audioTrack.Pause();
            _audioTrack.Flush();
            _audioTrack.Stop();
        }
    }

    private void HandleTimer()
    {
        if (_timerEventCounter == 1)
        {
            if (!IMetronomeService.MuteOverride && !_liveModeStarted)
                _audioTrack.Write(_beatBuffer, 0, _beatBuffer.Length, WriteMode.NonBlocking);
            if (_useHaptic && _canVibrate)
                Vibrator?.Vibrate(_beatEffect);
            MainThread.BeginInvokeOnMainThread(() => IMetronomeService.BeatAction());

            if (IMetronomeService.LiveMode && !_liveModeStarted)
            {
                _beatsPlayed++;
                if (_beatsPlayed >= IMetronomeService.BeatsPerMeasure)
                    _liveModeStarted = true;
            }
        }
        else
        {
            if (_playSubdivisions)
            {
                if (!IMetronomeService.MuteOverride && !_liveModeStarted)
                    _audioTrack.Write(_rhythmBuffer, 0, _rhythmBuffer.Length, WriteMode.NonBlocking);
                if (_useHaptic && _canVibrate)
                    Vibrator?.Vibrate(_rhythmEffect);
            }
            MainThread.BeginInvokeOnMainThread(() =>
                IMetronomeService.RhythmAction());
        }

        _timerEventCounter++;
        if (_timerEventCounter > _subdivisions)
            _timerEventCounter = 1;
    }

    public double GetMillisecondsPerBeat()
    {
        return 60000 / _bpm;
    }
}

public class MetronomeTimerTask : TimerTask
{
    public Action Action;
    public override void Run()
    {
        Action();
    }
}

