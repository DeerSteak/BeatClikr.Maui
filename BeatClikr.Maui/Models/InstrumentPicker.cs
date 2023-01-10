namespace BeatClikr.Maui.Models;

public class InstrumentPicker
{
    public string Title { get; set; }
    public string Value { get; set; }
    public bool IsBeat { get; set; }
    public bool IsRhythm { get; set; }

    public InstrumentPicker()
    {
        Title = string.Empty;
        Value = string.Empty;
    }

    public override string ToString()
    {
        return Title;
    }
}

