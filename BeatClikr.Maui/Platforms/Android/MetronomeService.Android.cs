using Android.Content;
using Android.Media;
using Android.OS;
using Android.Preferences;
using BeatClikr.Maui.Services.Interfaces;
using Java.Text;
using Java.Util;
using Java.Util.Concurrent;

namespace BeatClikr.Maui.Platforms.Android;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]

public class MetronomeService : IMetronomeService
{
  private int _bpm;
  private int _subdivisions;
  private bool _playSubdivisions;
  private int _subdivisionLengthInSamples;
  private bool _useFlashlight;
  private const int SAMPLE_RATE = 44100;
  private const int WAV_BUFFER_OFFSET = 44;
  private ScheduledThreadPoolExecutor _timer;
  private int _timerEventCounter;
  private int _beatsPlayed;
  private bool _liveModeStarted;
  private bool _previouslySetup = false;

  private byte[] _beatBuffer;
  private byte[] _rhythmBuffer;
  private readonly static byte[] _silenceBuffer = SetSilenceBuffer();
  private readonly int _minBufferSize;

  private string _beatFilename;
  private string _rhythmFilename;

  private bool _isPlaying;
  private AudioTrack _audioTrack;
  double _subdivisionLengthInMilliseconds;
  private bool _useHaptic;
  private VibrationEffect _beatEffect;
  private VibrationEffect _rhythmEffect;
  private bool _canVibrate;


  static VibratorManager? VibratorManager =>
          OperatingSystem.IsAndroidVersionAtLeast(31)
              ? MainApplication.Context.GetSystemService(Context.VibratorManagerService) as VibratorManager
              : null;

  static Vibrator VibratorManagerVibrator =>
      OperatingSystem.IsAndroidVersionAtLeast(31)
          ? VibratorManager?.DefaultVibrator
          : null;

  static Vibrator? VibratorServiceVibrator =>
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CA1422 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
      MainApplication.Context.GetSystemService(Context.VibratorService) as Vibrator;
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning restore CS0618 // Type or member is obsolete

  static Vibrator vibrator;

  static Vibrator Vibrator =>
      vibrator ??= (VibratorManagerVibrator ?? VibratorServiceVibrator);


  public MetronomeService()
  {
    _minBufferSize = AudioTrack.GetMinBufferSize(SAMPLE_RATE, ChannelOut.Stereo, Encoding.Pcm16bit);
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
    using (MemoryStream memoryStream = new MemoryStream())
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
    using (MemoryStream memoryStream = new MemoryStream())
    {
      fileStream.CopyTo(memoryStream);
      var buffer = memoryStream.GetBuffer();
      return buffer;
    }
  }

  public void SetTempo(int bpm, int subdivisions)
  {
    switch (bpm)
    {
      case < 60:
        _bpm = 60;
        break;
      case > 240:
        _bpm = 240;
        break;
      default:
        _bpm = bpm;
        break;
    }

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
    _subdivisionLengthInSamples = (int)(_subdivisionLengthInMilliseconds * SAMPLE_RATE / 1000D);
  }

  private void StartTimer()
  {
    _audioTrack.SetVolume(0);
    var bytesWritten = _audioTrack.Write(_silenceBuffer, 0, _silenceBuffer.Length, WriteMode.NonBlocking);
    _audioTrack.Play();
    _audioTrack.SetVolume(.9f);

    var task = new MetronomeTimerTask();
    task.Action = HandleTimer;

    _timer = new ScheduledThreadPoolExecutor(_subdivisions);
    _timer.ScheduleAtFixedRate(task, 0, (long)_subdivisionLengthInMilliseconds, TimeUnit.Milliseconds);
  }

  private void SetupVibrator()
  {
    _useHaptic = Preferences.Get(PreferenceKeys.UseHaptic, false);
    _canVibrate = OperatingSystem.IsAndroidVersionAtLeast(29);
    if (_useHaptic && _canVibrate)
    {
      _beatEffect = VibrationEffect.CreatePredefined(VibrationEffect.EffectHeavyClick);
      _rhythmEffect = VibrationEffect.CreatePredefined(VibrationEffect.EffectClick);
    }
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
    //do all the beat stuff. 
    if (_timerEventCounter == 1)
    {
      if (!IMetronomeService.MuteOverride
          && !_liveModeStarted)
      {
        _audioTrack.Write(_beatBuffer, 0, _beatBuffer.Length, WriteMode.NonBlocking);
      }
      MainThread.BeginInvokeOnMainThread(() =>
          IMetronomeService.BeatAction());
      if (_useHaptic && _canVibrate)
        Vibrator?.Vibrate(_beatEffect);

      if (IMetronomeService.LiveMode && !_liveModeStarted)
      {
        _beatsPlayed++;
        if (_beatsPlayed >= IMetronomeService.BeatsPerMeasure)
          _liveModeStarted = true;
      }
    }
    else if (!_playSubdivisions)
    {
      MainThread.BeginInvokeOnMainThread(() =>
          IMetronomeService.RhythmAction());
    }
    //if it's time to play a rhythm sound
    else
    {
      if (!IMetronomeService.MuteOverride && !_liveModeStarted)
        _audioTrack.Write(_rhythmBuffer, 0, _rhythmBuffer.Length, WriteMode.NonBlocking);
      if (_useHaptic && _canVibrate)
        Vibrator?.Vibrate(_rhythmEffect);
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

