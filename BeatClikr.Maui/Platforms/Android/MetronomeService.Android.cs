using Android.Media;
using Android.Preferences;
using BeatClikr.Maui.Services.Interfaces;
using Java.Text;
using Java.Util;

namespace BeatClikr.Maui.Platforms.Android;

public class MetronomeService : IMetronomeService
{
    private int _bpm;
    private int _subdivisions;
    private bool _playSubdivisions;
    private int _subdivisionLengthInSamples;
    private bool _useFlashlight;
    private const int SAMPLE_RATE = 44100;
    private IDispatcherTimer _timer;
    private int _timerEventCounter;
    private int _beatsPlayed;
    private bool _liveModeStarted;
    private bool _previouslySetup = false;

    private byte[] _beatBuffer;
    private byte[] _rhythmBuffer;

    private string _beatFilename;
    private string _rhythmFilename;

    private SoundPool _soundPool;
    private bool _isPlaying;
    

    public MetronomeService()
    {
    }

    public void Play()
    {
        _timerEventCounter = 1;
        _beatsPlayed = 0;
        _liveModeStarted = false;
        StartTimer();
    }

    public void SetBeat(string fileName, string set)
    {
        WriteFileToCache(fileName, set);
        _beatFilename = fileName;
    }

    //private byte[] SetBuffer(string fileName, string set)
    //{
    //    byte[] bytes;
    //    using System.IO.Stream fileStream = FileSystem.Current.OpenAppPackageFileAsync($"{set}/{fileName}.wav").Result;
    //    using (MemoryStream memoryStream = new MemoryStream())
    //    {
    //        fileStream.CopyTo(memoryStream);
    //        bytes = memoryStream.GetBuffer();
    //    }

    //    if (bytes == null)
    //    {
    //        throw new NullReferenceException($"Could not read {fileName}.wav into buffer.");
    //    }

    //    return bytes;
    //}

    public void SetRhythm(string fileName, string set)
    {
        WriteFileToCache(fileName, set);
        _rhythmFilename = fileName;
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

        _subdivisionLengthInSamples = (int)((60.0 / (_bpm * _subdivisions)) * SAMPLE_RATE);
    }

    private void StartTimer()
    {
        var timerIntervalInSamples = 0.5 * _subdivisionLengthInSamples / SAMPLE_RATE;

        _timer = Application.Current.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(timerIntervalInSamples);
        _timer.Tick += (s, e) => HandleTimer();
        _timer.Start();

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

        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            _soundPool = new SoundPool.Builder()
                .SetMaxStreams(_subdivisions * 2)
                .SetAudioAttributes(new AudioAttributes.Builder()
                    .SetFlags(AudioFlags.AudibilityEnforced)
                    .SetUsage(AudioUsageKind.Media)
                    .SetContentType(AudioContentType.Sonification)
                    .Build())
                .Build();
            var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.ApplicationContext;
            var res = context.Resources;

            _soundPool.Load(Path.Combine(FileSystem.Current.AppDataDirectory, $"{_beatFilename}.wav"), 1);
            for (int i = 0; i < _subdivisions * 2; i++)
            {
                _soundPool.Load(Path.Combine(FileSystem.Current.AppDataDirectory, $"{_rhythmFilename}.wav"), 1);
            }
        }

        _previouslySetup = true;
    }

    public void Stop()
    {
        if (_timer != null)
            _timer.Stop();
    }

    private void HandleTimer()
    {
        if (_timerEventCounter == 1)
        {
            if (!IMetronomeService.MuteOverride && !_liveModeStarted)
            {
                //_playerNode.ScheduleBuffer(_beatBuffer, null);
                _soundPool.Play(_timerEventCounter, 1f, 1f, 1, 0, 1);
            }
            //bright bulb icon and light up flashlight
            MainThread.BeginInvokeOnMainThread(() =>
                IMetronomeService.BeatAction());
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
                _soundPool.Play(_timerEventCounter, 1f, 1f, 1, 0, 1);
            }
            Console.WriteLine("Playing subdivision");
        }

        else if (!_playSubdivisions || _subdivisions == _timerEventCounter)
        {
            //dim bulb and turn off flashlight, for example
            MainThread.BeginInvokeOnMainThread(() => 
                IMetronomeService.RhythmAction());
        }

        _timerEventCounter++;
        if (_timerEventCounter > _subdivisions * 2)
            _timerEventCounter = 1;
    }

    private void WriteFileToCache(string fileName, string set)
    {
        string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, $"{fileName}.wav");
        if (File.Exists(targetFile))
            return;

        // Read the source file
        using System.IO.Stream fileStream = FileSystem.Current.OpenAppPackageFileAsync($"{set}/{fileName}.wav").Result;
        using MemoryStream memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);

        // Write the file content to the app data directory
        
        using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
        memoryStream.WriteTo(outputStream);
        outputStream.Close();
    }
}

