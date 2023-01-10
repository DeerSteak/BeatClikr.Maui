using SQLite;

namespace BeatClikr.Maui.Models;

public class RehearsalPlaylistEntry
{
    public RehearsalPlaylistEntry()
    {
    }

    [PrimaryKey, AutoIncrement]
    public int? ID { get; set; }

    public int SongID { get; set; }
}

