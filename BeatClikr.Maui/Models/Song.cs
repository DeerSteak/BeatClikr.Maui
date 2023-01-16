using BeatClikr.Maui.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace BeatClikr.Maui.Models;

public partial class Song : ObservableObject
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string artist;

    [ObservableProperty]
    private int beatsPerMinute;

    [ObservableProperty]
    private SubdivisionEnum subdivision;

    [ObservableProperty]
    private int beatsPerMeasure;

    [ObservableProperty]
    private int? liveSequence;

    [ObservableProperty]
    private int? rehearsalSequence;

    [PrimaryKey, AutoIncrement]
    public int? Id { get; set; }

    public Song()
    {
        Title = string.Empty;
        Artist = string.Empty;
        BeatsPerMinute = 60;
        BeatsPerMeasure = 4;
        Subdivision = SubdivisionEnum.Eighth;
        LiveSequence = null;
        RehearsalSequence = null;
    }

    public override string ToString()
    {
        return $"{Title} by {Artist}: {BeatsPerMinute} bpm";
    }

    public static Song Instance = null;
}

