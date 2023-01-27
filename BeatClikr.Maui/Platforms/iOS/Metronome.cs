﻿using System;
using AVFoundation;
using BeatClikr.Maui.Services.Interfaces;
using Foundation;

namespace BeatClikr.Maui.Platforms
{
	public class Metronome : IMetronome
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
        private const double SAMPLE_RATE = 44100;

        private readonly AVAudioEngine _avAudioEngine;
        private readonly AVAudioPlayerNode _playerNode;

        private int _currentBeat = 1;
        private int _timerEventCounter = 1;
        private Foundation.NSTimer _timer = null;
        private bool _useFlashlight = true;
        private bool _previouslySetup = false;

		public Metronome()
		{
            _avAudioEngine = new AVAudioEngine();
            SetTempo(_bpm, _subdivisions);
            _playerNode = new AVAudioPlayerNode();
            _avAudioEngine.AttachNode(_playerNode);
        }

        public void SetupMetronome(string beatFileName, string rhythmFileName, string set)
        {
            if (_previouslySetup)
                return;
            SetBeat(beatFileName, set);
            SetRhythm(rhythmFileName, set);
            SetTempo(_bpm, _subdivisions);
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
                Console.WriteLine(ex);
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

        public void Play()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            var timerIntervalInSamples = 0.5 * _subdivisionLengthInSamples / SAMPLE_RATE;
            _timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromSeconds(timerIntervalInSamples), (timer) =>
            {
                if (_timerEventCounter == 1)
                {
                    //schedule beat
                    _playerNode.ScheduleBuffer(_beatBuffer, null, AVAudioPlayerNodeBufferOptions.Interrupts, null);
                    IMetronome.BeatAction();
                    Console.WriteLine("Playing beat");
                    
                }
                else if (_timerEventCounter % 2 == 1)
                {
                    //schedule rhythm
                    _playerNode.ScheduleBuffer(_rhythmBuffer, null, AVAudioPlayerNodeBufferOptions.Interrupts, null);
                    IMetronome.RhythmAction();
                    Console.WriteLine("Playing subdivision");
                }
                else
                {
                    //do something else                    
                }

                _timerEventCounter++;
                if (_timerEventCounter > _subdivisions * 2)
                    _timerEventCounter = 1;
            });            
        }


        public void Stop()
        {
            if (_timer != null)
                _timer.Invalidate();
        }

        public void SetFlashlight(bool useFlashlight)
        {
            _useFlashlight = useFlashlight;
        }
    }
}

