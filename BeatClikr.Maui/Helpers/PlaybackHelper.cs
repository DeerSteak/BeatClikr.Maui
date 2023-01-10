using System.Reflection;
using BeatClikr.Maui.Enums;
namespace BeatClikr.Maui.Helpers
{
    public static class PlaybackUtilities
    {
        public static int GetSubdivisionsPerBeat(SubdivisionEnum sub)
        {
            switch (sub)
            {
                case SubdivisionEnum.Quarter:                    
                    return 2; // this is required to get the light to turn off while blinking
                case SubdivisionEnum.Eighth:
                    return 2;
                case SubdivisionEnum.TripletEighth:
                    return 3;
                case SubdivisionEnum.Sixteenth:
                    return 4;
                default: // should never happen (?!)
                    return 2;
            }
        }

        public static float GetTimerInterval(SubdivisionEnum sub, int bpm)
        {
            var subdivisionsPerBeat = GetSubdivisionsPerBeat(sub);
            var baseInterval = (1000F / (float)bpm) * 60;
            var timerInterval = baseInterval / (float)subdivisionsPerBeat;
            return timerInterval;
        }

        public static Stream GetStreamFromFile(string filename, string set)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var fileString = $"BeatClikr.Maui.Resources.Sounds.{set}.{filename}.m4a";
            var stream = assembly.GetManifestResourceStream(fileString);
            return stream;
        }
    }
}

