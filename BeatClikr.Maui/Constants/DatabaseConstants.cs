namespace BeatClikr.Maui.Constants;

public static class DatabaseConstants
{
    public const string DatabaseFilename = "livemetronome.db";

    public const SQLite.SQLiteOpenFlags Flags =
        SQLite.SQLiteOpenFlags.ReadWrite |
        SQLite.SQLiteOpenFlags.Create |
        SQLite.SQLiteOpenFlags.SharedCache;

    public static readonly string DatabasePath = Path.Combine(
                                            Environment.GetFolderPath
                                                (System.Environment.SpecialFolder.LocalApplicationData),
                                            DatabaseFilename);

}