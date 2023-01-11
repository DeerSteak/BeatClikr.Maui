using BeatClikr.Maui.Constants;
using BeatClikr.Maui.Models;
using BeatClikr.Maui.Services.Interfaces;
using SQLite;

namespace BeatClikr.Maui.Services;

public class DataService : IDataService
{
    SQLiteAsyncConnection db;
    public DataService()
    {
        db = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath);
    }

    public async Task<Song> GetById(int id)
    {
        await db.CreateTableAsync<Song>();
        return await db.GetAsync<Song>(id);
    }

    public async Task<List<Song>> GetLibrarySongs()
    {
        await db.CreateTableAsync<Song>();
        return await db.Table<Song>().ToListAsync();
    }

    public async Task<List<Song>> GetLiveSongs()
    {
        await db.CreateTableAsync<Song>();
        var items = await db.Table<Song>()
            .Where(s => s.LiveSequence != null)
            .OrderBy(s => s.LiveSequence)
            .ToListAsync();
        return items ?? new List<Song>();
    }

    public async Task<List<Song>> GetRehearsalSongs()
    {
        await db.CreateTableAsync<Song>();
        var items = await db.Table<Song>()
            .Where(s => s.RehearsalSequence != null)
            .OrderBy(s => s.RehearsalSequence)
            .ToListAsync();
        return items ?? new List<Song>();
    }

    public async Task<int> SaveSongListToLibrary(List<Song> songs)
    {
        var total = 0;
        foreach (var song in songs)
            total += await SaveSongToLibrary(song);
        return total;
    }

    public async Task<int> SaveSongToLibrary(Song song)
    {
        return await db.InsertOrReplaceAsync(song);
    }
}

