namespace BeatClikr.Maui.Models;

public class InstrumentPicker
{
    public string Title { get; set; }
    public string FileName { get; set; }
    public bool IsBeat { get; set; }
    public bool IsRhythm { get; set; }

    public InstrumentPicker()
    {
        Title = string.Empty;
        FileName = string.Empty;
    }

    public override string ToString()
    {
        return Title;
    }

    public static readonly List<InstrumentPicker> Instruments = new List<InstrumentPicker>()
    {
        new InstrumentPicker { Title = "Click High", FileName = FileNames.ClickHi, IsBeat = true, IsRhythm = true },
        new InstrumentPicker { Title = "Click Low", FileName = FileNames.ClickLo, IsBeat = true, IsRhythm = true },
        new InstrumentPicker { Title = "Cowbell",FileName = FileNames.Cowbell, IsBeat = true, IsRhythm = true },
        new InstrumentPicker { Title = "Crash 1", FileName = FileNames.CrashL, IsBeat = true, IsRhythm = false },
        new InstrumentPicker { Title = "Crash 2", FileName = FileNames.CrashR, IsBeat = true, IsRhythm = false },
        new InstrumentPicker { Title = "Hat Closed", FileName = FileNames.HatClosed, IsBeat = true, IsRhythm = true },
        new InstrumentPicker { Title = "Hat Open", FileName = FileNames.HatOpen, IsBeat = true, IsRhythm = true },
        new InstrumentPicker { Title = "Kick", FileName = FileNames.Kick, IsBeat = true, IsRhythm = false },
        new InstrumentPicker { Title = "Ride Bell", FileName = FileNames.RideBell, IsBeat = true, IsRhythm = false },
        new InstrumentPicker { Title = "Ride Edge", FileName = FileNames.Ride, IsBeat = true, IsRhythm = true },
        new InstrumentPicker { Title = "Snare", FileName = FileNames.Snare, IsBeat = true, IsRhythm = true },
        new InstrumentPicker { Title = "Tambourine", FileName = FileNames.Tamb, IsBeat = true, IsRhythm = true },
        new InstrumentPicker { Title = "Tom High", FileName = FileNames.TomHigh, IsBeat = true, IsRhythm = false },
        new InstrumentPicker { Title = "Tom Low", FileName = FileNames.TomLow, IsBeat = true, IsRhythm = false },
        new InstrumentPicker { Title = "Tom Middle", FileName = FileNames.TomMid, IsBeat = true, IsRhythm = false }        
    };

    public static InstrumentPicker FromString(string fileName)
    {
        var instrument = Instruments.FirstOrDefault(x => x.FileName == fileName);
        if (instrument != null)
            return instrument;

        return Instruments.First();
    }
}

