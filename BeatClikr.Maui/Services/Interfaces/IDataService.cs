using BeatClikr.Maui.Models;
namespace BeatClikr.Maui.Services.Interfaces
{
	public interface IDataService
	{
        List<Song> GetLibrarySongs();
        List<Song> GetLibrarySongs(string filter);
        int SaveSongToLibrary(Song song);
        int SaveSongListToLibrary(List<Song> songs);
        List<Song> GetLiveSongs();
        List<Song> GetRehearsalSongs();
        Song GetById(int id);
    }
}

