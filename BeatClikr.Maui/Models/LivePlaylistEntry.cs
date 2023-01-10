using SQLite;

namespace BeatClikr.Maui.Models;

public class LivePlaylistEntry
{
    public LivePlaylistEntry()
    {
    }

    [PrimaryKey, AutoIncrement]
    public int? ID { get; set; }

    public int SongID { get; set; }
}

