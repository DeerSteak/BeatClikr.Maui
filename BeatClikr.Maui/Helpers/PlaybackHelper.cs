namespace BeatClikr.Maui.Helpers;

public static class PlaybackUtilities
{
    public static int GetSubdivisionsPerBeat(Subdivisions sub)
    {
        switch (sub)
        {
            case Subdivisions.Quarter:
                return 2; // this is required to get the light to turn off while blinking
            case Subdivisions.Eighth:
                return 2;
            case Subdivisions.TripletEighth:
                return 3;
            case Subdivisions.Sixteenth:
                return 4;
            default: // should never happen (?!)
                return 2;
        }
    }

    public static float GetTimerInterval(Subdivisions sub, int bpm)
    {
        var subdivisionsPerBeat = GetSubdivisionsPerBeat(sub);
        var baseInterval = (1000F / (float)bpm) * 60;
        var timerInterval = baseInterval / (float)subdivisionsPerBeat;
        return timerInterval;
    }

    public static async Task<Stream> GetStreamFromFile(string filename, string set)
    {
        return await FileSystem.OpenAppPackageFileAsync($"{set}/{filename}.wav");
    }

    public static string GetFilePath(string filename, string set)
    {
        return $"{set}/{filename}.wav";
    }

}

