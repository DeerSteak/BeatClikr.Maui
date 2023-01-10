namespace BeatClikr.Maui.Constants;

public static class DatabaseConstants
{
    public const string DatabaseFilename = "livemetronome.db";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string DatabasePath = GetDbPath();

    private static string GetDbPath()
    {
#if IOS || MACCATALYST
        string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(dbPath, DatabaseFilename);
#else
        string dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        return Path.Combine(dbPath, DatabaseFilename);
#endif
    }
}