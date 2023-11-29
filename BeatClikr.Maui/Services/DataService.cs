using BeatClikr.Maui.Models;
using BeatClikr.Maui.Services.Interfaces;
using SQLite;

namespace BeatClikr.Maui.Services;

public class DataService : IDataService
{
    private readonly Lazy<SQLiteConnection> _synchronousConnectionHolder;
    protected SQLiteConnection Database
    {
        get => _synchronousConnectionHolder.Value;
    }

    public DataService()
    {
        _synchronousConnectionHolder = new Lazy<SQLiteConnection>(() =>
            new SQLiteConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags));
    }

    public Song GetById(int id)
    {
        Database.CreateTable<Song>();
        return Database.Get<Song>(id);
    }

    public List<Song> GetLibrarySongs()
    {
        var table = Database.CreateTable<Song>();
        return Database.Table<Song>().OrderBy(x => x.Title).ThenBy(x => x.Artist).ToList();
    }

    public List<Song> GetLibrarySongs(string filter)
    {
        if (string.IsNullOrEmpty(filter))
            return GetLibrarySongs();

        var toLower = filter.ToLower();

        Database.CreateTable<Song>();
        var list = Database.Table<Song>().Where(
                x => x.Artist.ToLower().Contains(toLower)
                || x.Title.ToLower().Contains(toLower))
            .OrderBy(x => x.Title).ThenBy(x => x.Artist)
            .ToList();
        return list;
    }

    public List<Song> GetLiveSongs()
    {
        Database.CreateTable<Song>();
        var items = Database.Table<Song>()
            .Where(s => s.LiveSequence != null)
            .OrderBy(s => s.LiveSequence)
            .ToList();
        return items ?? new List<Song>();
    }

    public List<Song> GetRehearsalSongs()
    {
        Database.CreateTable<Song>();
        var items = Database.Table<Song>()
            .Where(s => s.RehearsalSequence != null)
            .OrderBy(s => s.RehearsalSequence)
            .ToList();
        return items ?? new List<Song>();
    }

    public int SaveSongListToLibrary(List<Song> songs)
    {
        var total = 0;
        foreach (var song in songs)
            total += SaveSongToLibrary(song);
        return total;
    }

    public int SaveSongToLibrary(Song song)
    {
        Database.CreateTable<Song>();
        return Database.InsertOrReplace(song);
    }
}

