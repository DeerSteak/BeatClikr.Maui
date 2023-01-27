using System;
namespace BeatClikr.Maui.Services.Interfaces;

public interface IMetronome
{
    void Play();
    void Stop();
    void SetTempo(int bpm, int subdivisions);
    void SetBeat(string fileName, string set);
    void SetRhythm(string fileName, string set);
    void SetupMetronome(string beatFileName, string rhythmFileName, string set);
    static Action BeatAction;
    static Action RhythmAction;
}

