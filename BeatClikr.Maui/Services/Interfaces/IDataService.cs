using BeatClikr.Maui.Models;
namespace BeatClikr.Maui.Services.Interfaces
{
	public interface IDataService
	{
        Task<List<Song>> GetLibrarySongs();
        Task<int> SaveSongToLibrary(Song song);
        Task<int> SaveSongListToLibrary(List<Song> songs);
        Task<List<Song>> GetLiveSongs();
        Task<List<Song>> GetRehearsalSongs();
        Task<Song> GetById(int id);
    }
}

