using AVFoundation;
using BeatClikr.Maui.Services.Interfaces;
using Foundation;
using UIKit;

namespace BeatClikr.Maui.Platforms.iOS;

public class MetronomeService : IMetronomeService
{
    private Uri _beatUri;
    private Uri _rhythmUri;
    private bool _playSubdivisions;

    private AVAudioFile _beatFile;
    private AVAudioFile _rhythmFile;

    private AVAudioPcmBuffer _beatBuffer;
    private AVAudioPcmBuffer _rhythmBuffer;

    private int _bpm = 60;
    private int _subdivisions = 1;

    private int _subdivisionLengthInSamples;
    private double _subdivisionLengthInMilliseconds;
    private const double SAMPLE_RATE = 44100;

    private readonly AVAudioEngine _avAudioEngine;
    private readonly AVAudioPlayerNode _playerNode;

    private bool _liveModeStarted = false;
    private int _beatsPlayed = 0;

    private int _timerEventCounter = 1;
    private Foundation.NSTimer _timer = null;
    private bool _useFlashlight = true;
    private bool _previouslySetup = false;

    private bool _useHaptic = false;
    UIImpactFeedbackGenerator _beatGenerator;
    UIImpactFeedbackGenerator _rhythmGenerator;

    AVCaptureDevice _device;

    public MetronomeService()
    {
        _avAudioEngine = new AVAudioEngine();
        SetTempo(_bpm, _subdivisions);
        _playerNode = new AVAudioPlayerNode();
        _avAudioEngine.AttachNode(_playerNode);
        _device = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);
    }

    public void SetupMetronome(string beatFileName, string rhythmFileName, string set)
    {
        SetBeat(beatFileName, set);
        SetRhythm(rhythmFileName, set);
        SetUseFlashlight();
        SetHaptic();

        if (_previouslySetup)
            return;
        try
        {
            var format = _beatFile.ProcessingFormat.StreamDescription;
            _avAudioEngine.AttachNode(_playerNode);
            _avAudioEngine.Connect(sourceNode: _playerNode, targetNode: _avAudioEngine.MainMixerNode, format: _beatFile.ProcessingFormat);
            _avAudioEngine.Prepare();
            _avAudioEngine.StartAndReturnError(out var startError);
            if (startError != null)
            {

            }
            _previouslySetup = true;
        }
        catch (Exception ex)
        {

        }
    }

    public void SetBeat(string fileName, string set)
    {
        try
        {
            var uris = NSBundle.MainBundle.GetUrlsForResourcesWithExtension($".wav", set);
            _beatUri = uris.FirstOrDefault(x => x.ToString().Contains(fileName));
            _beatFile = new AVAudioFile(_beatUri, out var fileError);
            if (fileError != null)
            {

            }
            _beatBuffer = new AVAudioPcmBuffer(_beatFile.ProcessingFormat, (uint)(_subdivisionLengthInSamples));
            _beatFile.ReadIntoBuffer(_beatBuffer, out var bufferError);
            if (bufferError != null)
            {

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void SetRhythm(string fileName, string set)
    {
        try
        {
            var uris = NSBundle.MainBundle.GetUrlsForResourcesWithExtension($".wav", set);
            _rhythmUri = uris.FirstOrDefault(x => x.ToString().Contains(fileName));
            _rhythmFile = new AVAudioFile(_rhythmUri, out var fileError);
            if (fileError != null)
            {

            }
            _rhythmBuffer = new AVAudioPcmBuffer(_rhythmFile.ProcessingFormat, (uint)_subdivisionLengthInSamples);
            _rhythmFile.ReadIntoBuffer(_rhythmBuffer, out var bufferError);
            if (bufferError != null)
            {

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void SetTempo(int bpm, int subdivisions)
    {
        switch (bpm)
        {
            case < 30:
                _bpm = 30;
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

    public void Play()
    {
        _timerEventCounter = 1;
        _beatsPlayed = 0;
        _liveModeStarted = false;
        _playerNode.Stop();
        _playerNode.Play();
        StartTimer();
    }

    private void StartTimer()
    {
        var timerIntervalInSamples = _subdivisionLengthInSamples / SAMPLE_RATE;
        _timer = NSTimer.CreateRepeatingScheduledTimer(
            TimeSpan.FromMilliseconds(_subdivisionLengthInMilliseconds), (timer) => HandleTimer());
    }

    private void HandleTimer()
    {
        if (_timerEventCounter == 1)
        {
            if (!IMetronomeService.MuteOverride && !_liveModeStarted)
                _playerNode.ScheduleBuffer(_beatBuffer, null);
            if (_useHaptic)
                VibrateBeat();
            if (_useFlashlight)
                ToggleFlashlight(true);

            IMetronomeService.BeatAction();
            if (IMetronomeService.LiveMode && !_liveModeStarted)
            {
                _beatsPlayed++;
                if (_beatsPlayed >= IMetronomeService.BeatsPerMeasure)
                    _liveModeStarted = true;
            }
        }
        else //subdivision
        {
            if (_playSubdivisions)
            {
                if (!IMetronomeService.MuteOverride && !_liveModeStarted)
                    _playerNode.ScheduleBuffer(_rhythmBuffer, null);
                if (_useHaptic)
                    VibrateRhythm();
            }

            if (_useFlashlight)
                ToggleFlashlight(false);

            IMetronomeService.RhythmAction();
        }

        _timerEventCounter++;
        if (_timerEventCounter > _subdivisions)
            if (_timerEventCounter > _subdivisions)
                _timerEventCounter = 1;
    }

    public void Stop()
    {
        if (_timer != null)
            _timer.Invalidate();
        ToggleFlashlight(false);
    }

    void SetUseFlashlight()
    {
        _useFlashlight = Preferences.Get(PreferenceKeys.UseFlashlight, false);
        if (_device != null && _useFlashlight)
        {
            _useFlashlight = _device.HasTorch;
        }
    }

    void ToggleFlashlight(bool on)
    {
        if (_device == null || !(_device.HasFlash || _device.HasTorch))
            return;

        _device.LockForConfiguration(out var error);

        if (error == null)
        {
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1422 // Validate platform compatibility
            if (on)
            {
                if (_device.HasTorch)
                    _device.SetTorchModeLevel(AVCaptureDevice.MaxAvailableTorchLevel, out var torchErr);
                else if (_device.HasFlash)
                    _device.FlashMode = AVCaptureFlashMode.On;
            }
            else
            {
                if (_device.HasTorch)
                    _device.TorchMode = AVCaptureTorchMode.Off;
                else if (_device.HasFlash)
                    _device.FlashMode = AVCaptureFlashMode.Off;
            }
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
        }

        _device.UnlockForConfiguration();
    }

    void SetHaptic()
    {
        _useHaptic = Preferences.Get(PreferenceKeys.UseHaptic, false);
        _beatGenerator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
        _rhythmGenerator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
    }

    void VibrateBeat()
    {
        if (_beatGenerator != null)
            _beatGenerator.ImpactOccurred();
    }

    void VibrateRhythm()
    {
        if (_rhythmGenerator != null)
            _rhythmGenerator.ImpactOccurred();
    }

    public double GetMillisecondsPerBeat()
    {
        return 60000 / _bpm;
    }
}
