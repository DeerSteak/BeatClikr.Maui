namespace BeatClikr.Maui.Services.Interfaces;

public interface IMetronomeService
{
    void Play();
    void Stop();
    void SetTempo(int bpm, int subdivisions);
    void SetBeat(string fileName, string set);
    void SetRhythm(string fileName, string set);
    void SetupMetronome(string beatFileName, string rhythmFileName, string set);
    static Action BeatAction { get; set; }
    static Action RhythmAction { get; set; }
    static bool LiveMode { get; set; }
    static bool MuteOverride { get; set; }
    static int BeatsPerMeasure { get; set; } = 4;
    bool SupportsLowLatency();
    double GetMillisecondsPerBeat();
}

