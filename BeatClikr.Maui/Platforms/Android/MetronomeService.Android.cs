using Android.Bluetooth;
using BeatClikr.Maui.Services.Interfaces;
using Java.Nio.Channels;
using Java.Text;
using Java.Util;
using Org.Apache.Http.Conn;

namespace BeatClikr.Maui.Platforms.Android;

public class MetronomeService : IMetronomeService
{
    private int _bpm;
    private int _subdivisions;
    private bool _playSubdivisions;
    private int _subdivisionLengthInSamples;
    private bool _useFlashlight;
    private const int SAMPLE_RATE = 44100;
    private Java.Util.Timer _timer;
    private int _timerEventCounter;
    private int _beatsPlayed;
    private bool _liveModeStarted;
    private bool _previouslySetup = false;

    public MetronomeService()
    {
    }

    public void Play()
    {
        _timerEventCounter = 1;
        _beatsPlayed = 0;
        _liveModeStarted = false;        
    }

    public void SetBeat(string fileName, string set)
    {
        //get URI of embedded resource file
        //Load file 
        //Create buffer from file
    }

    public void SetFlashlight(bool useFlashlight)
    {
        _useFlashlight = useFlashlight;
    }

    public void SetRhythm(string fileName, string set)
    {
        //get URI of embedded resource file
        //Load file 
        //Create buffer from file
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
            case < 1:
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

        _subdivisionLengthInSamples = (int)((60.0 / (_bpm * _subdivisions)) * SAMPLE_RATE);
    }

    private void StartTimer()
    {
        var timerIntervalInSamples = 0.5 * _subdivisionLengthInSamples / SAMPLE_RATE;

                //_timer = NSTimer.CreateRepeatingScheduledTimer(
            //TimeSpan.FromSeconds(timerIntervalInSamples), (timer) => HandleTimer());
    }
    public static Date GetStartTime()
    {
        try
        {
            var oneSecondFromNow = DateTime.Now.AddSeconds(1);
            var startTime = oneSecondFromNow.ToString("yyyy-MM-dd:hh:mm:ss");
            return new SimpleDateFormat("yyyy-MM-dd:hh:mm:ss").Parse(startTime);
        }
        catch (ParseException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public void SetupMetronome(string beatFileName, string rhythmFileName, string set)
    {
        SetBeat(beatFileName, set);
        SetRhythm(rhythmFileName, set);
        SetTempo(_bpm, _subdivisions);

        if (_previouslySetup)
            return;

        //create audio player
        //prepare player for streaming
        //start the audio engine

        _previouslySetup = true;
    }

    public void Stop()
    {
        if (_timer != null)
            _timer.Cancel();
    }

    private void HandleTimer()
    {
        if (_timerEventCounter == 1)
        {
            if (!IMetronomeService.MuteOverride && !_liveModeStarted)
            {
                //_playerNode.ScheduleBuffer(_beatBuffer, null);
            }
            IMetronomeService.BeatAction();
            Console.WriteLine("Playing beat");
            if (IMetronomeService.LiveMode && !_liveModeStarted)
            {
                _beatsPlayed++;
                if (_beatsPlayed >= IMetronomeService.BeatsPerMeasure)
                    _liveModeStarted = true;
            }
        }
        else if (_timerEventCounter % 2 == 1)
        {
            if (!IMetronomeService.MuteOverride && _playSubdivisions && !_liveModeStarted)
            {
                //_playerNode.ScheduleBuffer(_rhythmBuffer, null);
            }
            Console.WriteLine("Playing subdivision");
        }

        if (_timerEventCounter == _subdivisions)
        {
            IMetronomeService.RhythmAction();
        }

        _timerEventCounter++;
        if (_timerEventCounter > _subdivisions * 2)
            _timerEventCounter = 1;
    }
}

