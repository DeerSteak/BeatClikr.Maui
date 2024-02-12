using Android.Content.PM;
using BeatClikr.Maui.Services.Interfaces;
using Java.Util;
using Java.Util.Concurrent;

namespace BeatClikr.Maui.Platforms.Droid;

public class MetronomeService : IMetronomeService, IDisposable
{
    private int _bpm;
    private int _subdivisions;
    private bool _playSubdivisions;
    private ScheduledThreadPoolExecutor _timer;
    private int _timerEventCounter;
    private int _beatsPlayed;
    private bool _liveModeStarted;
    private readonly SoundPoolNotePlayer _notePlayer;
    private string _beatFileName;
    private string _rhythmFileName;
    double _subdivisionLengthInMilliseconds;
    private readonly VibratorPlayer _vibratorPlayer;
 
    public MetronomeService()
    {        
        _vibratorPlayer = new VibratorPlayer();        
        _notePlayer = new SoundPoolNotePlayer();
        SetHaptic();
    }

    public void SetHaptic()
    {
        _vibratorPlayer.SetHaptic();
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
        _beatFileName = fileName;        
    }

    public void SetRhythm(string fileName, string set)
    {
        _rhythmFileName = fileName;        
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
        var task = new MetronomeTimerTask
        {
            Action = HandleTimer
        };
        _timer = new ScheduledThreadPoolExecutor(_subdivisions);
        _timer.ScheduleAtFixedRate(task, 0, (long)_subdivisionLengthInMilliseconds, TimeUnit.Milliseconds);
    }

    public void SetupMetronome(string beatFileName, string rhythmFileName, string set)
    {   
        _beatFileName = beatFileName;
        _rhythmFileName = rhythmFileName;

        SetTempo(_bpm, _subdivisions);

        _subdivisionLengthInMilliseconds = 60000D / (_bpm * _subdivisions);       
    }

    public void Stop() => _timer?.ShutdownNow();

    private void HandleTimer()
    {
        if (_timerEventCounter == 1)
        {
            if (!IMetronomeService.MuteOverride && !_liveModeStarted)
                _notePlayer.Play(_beatFileName);
            _vibratorPlayer.PlayBeat();
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
                    _notePlayer.Play(_rhythmFileName);
                _vibratorPlayer.PlayRhythm();
            }
            MainThread.BeginInvokeOnMainThread(() =>
                IMetronomeService.RhythmAction());
        }

        _timerEventCounter++;
        if (_timerEventCounter > _subdivisions)
            _timerEventCounter = 1;
    }

    public double GetMillisecondsPerBeat() => 60000 / _bpm;

    public void Dispose()
    {
        _notePlayer.Dispose();
        GC.SuppressFinalize(this);
    }

    public bool SupportsLowLatency()
    {
        var lla = Android.App.Application.Context.PackageManager.HasSystemFeature(PackageManager.FeatureAudioLowLatency);
        var pa = Android.App.Application.Context.PackageManager.HasSystemFeature(PackageManager.FeatureAudioPro);
        return pa || lla;
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