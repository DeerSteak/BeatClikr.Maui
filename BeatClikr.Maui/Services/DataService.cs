using BeatClikr.Maui.Constants;
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

    public async Task<Song> GetById(int id)
    {
        Database.CreateTable<Song>();
        return Database.Get<Song>(id);

        //var liteDb = new LiteDB.LiteDatabase(DatabaseConstants.DatabasePath);
        //var collection = liteDb.GetCollection<Song>();
        //return collection.FindById(id);
    }

    public async Task<List<Song>> GetLibrarySongs()
    {
        var table = Database.CreateTable<Song>();
        Console.WriteLine(table.ToString());
        return Database.Table<Song>().ToList();

        //var liteDb = new LiteDB.LiteDatabase(DatabaseConstants.DatabasePath);
        //var collection = liteDb.GetCollection<Song>().FindAll();
        //var list = collection.ToList();
        //return list;
    }

    public async Task<List<Song>> GetLibrarySongs(string filter)
    {
        if (string.IsNullOrEmpty(filter))
            return await GetLibrarySongs();

        var toLower = filter.ToLower();

        Database.CreateTable<Song>();
        var list = Database.Table<Song>().Where(
            x => x.Artist.ToLower().Contains(toLower)
            || x.Title.ToLower().Contains(toLower))
            .ToList();
        return list;
        //if (string.IsNullOrEmpty(filter))
        //    return await GetLibrarySongs();

        //var liteDb = new LiteDB.LiteDatabase(DatabaseConstants.DatabasePath);
        //var collection = liteDb.GetCollection<Song>()
        //                        .Find(
        //                            x => x.Artist.ToLower().Contains(filter.ToLower())
        //                            || x.Title.ToLower().Contains(filter.ToLower()));
        //var list = collection.OrderBy(x => x.Title)
        //                        .ThenBy(x => x.Artist)
        //                        .ToList();

        //return list;
    }

    public async Task<List<Song>> GetLiveSongs()
    {
        Database.CreateTable<Song>();
        var items = Database.Table<Song>()
            .Where(s => s.LiveSequence != null)
            .OrderBy(s => s.LiveSequence)
            .ToList();
        return items ?? new List<Song>();

        //var liteDb = new LiteDB.LiteDatabase(DatabaseConstants.DatabasePath);
        //var collection = liteDb.GetCollection<Song>()
        //    .Find(x => x.LiveSequence != null);
        //return collection.OrderBy(s => s.LiveSequence).ToList();
    }

    public async Task<List<Song>> GetRehearsalSongs()
    {
        Database.CreateTable<Song>();
        var items = Database.Table<Song>()
            .Where(s => s.RehearsalSequence != null)
            .OrderBy(s => s.RehearsalSequence)
            .ToList();
        return items ?? new List<Song>();

        //var liteDb = new LiteDB.LiteDatabase(DatabaseConstants.DatabasePath);
        //var collection = liteDb.GetCollection<Song>()
        //    .Find(x => x.RehearsalSequence != null);
        //return collection.OrderBy(s => s.LiveSequence).ToList();
    }

    public async Task<int> SaveSongListToLibrary(List<Song> songs)
    {
        var total = 0;
        foreach (var song in songs)
            total += await SaveSongToLibrary(song);
        return total;

        //var liteDb = new LiteDB.LiteDatabase(DatabaseConstants.DatabasePath);
        //var songCollection = liteDb.GetCollection<Song>();
        //return songCollection.Update(songs);
    }

    public async Task<int> SaveSongToLibrary(Song song)
    {
        Database.CreateTable<Song>();
        return Database.InsertOrReplace(song);

        //var liteDb = new LiteDB.LiteDatabase(DatabaseConstants.DatabasePath);
        //var songCollection = liteDb.GetCollection<Song>();
        //if (songCollection.Exists(x => x.Id == song.Id))
        //{
        //    songCollection.Insert(song);
        //    return 1;
        //}
        //return songCollection.Update(song) ? 1 : 0;
    }
}

