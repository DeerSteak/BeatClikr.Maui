using BeatClikr.Maui.Services.Interfaces;
using Xunit;

namespace BeatClikr.Maui.Test
{
    public class MetronomeServiceTests
    {
        [Fact]
        public void GetMillisecondsForSixtyBpm()
        {
            double millisecondPerBeat = 0;
            IMetronomeService metronomeSvc =
#if ANDROID
            new BeatClikr.Maui.Platforms.Android.MetronomeService();
#elif IOS || MACCATALYST
            new BeatClikr.Maui.Platforms.iOS.MetronomeService();
#endif
            metronomeSvc.SetTempo(60, 1);
            millisecondPerBeat = metronomeSvc.GetMillisecondsPerBeat();

            Assert.Equal(60, millisecondPerBeat);
        }
    }
}
