using Android.Content;
using Android.Media;
using Android.Text.Style;

namespace BeatClikr.Maui.Platforms.Droid
{
    public class SoundPoolNotePlayer : IDisposable
    {
        private readonly SoundPool _soundPool;
        private const int SAMPLE_RATE = 44100;
        private readonly Dictionary<string, int> _soundHandles = [];
        private readonly Context _appContext = Android.App.Application.Context;

        public SoundPoolNotePlayer()
        {
            _soundPool = new SoundPool.Builder()
                .SetMaxStreams(16)
                .SetAudioAttributes(GetAudioAttributes())
                .Build();
            LoadAllNotes();
        }

        private static AudioAttributes GetAudioAttributes()
        {
            var audioAttributesBuilder = new AudioAttributes.Builder()
                .SetUsage(AudioUsageKind.Game)
                .SetContentType(AudioContentType.Sonification);

            return audioAttributesBuilder.Build();
        }

        public void Play(string fileName, bool isBeat)
        {
            var volume = isBeat ? 1.5f : 1.0f;
            var soundHandle = GetSoundHandle(fileName);            
            _soundPool.Play(soundHandle, volume, volume, 1, 0, 1f);            
        }

        private void LoadNote(string fileName)
        {            
            var soundId = GetResourceDescriptor(fileName);
            _soundHandles.Remove(fileName);
            _soundHandles[fileName] = _soundPool.Load(_appContext, soundId, 1);            
        }

        public void LoadAllNotes()
        {
            LoadNote(FileNames.ClickHi);
            LoadNote(FileNames.ClickLo);
            LoadNote(FileNames.Cowbell);
            LoadNote(FileNames.CrashL);
            LoadNote(FileNames.CrashR);
            LoadNote(FileNames.HatClosed);
            LoadNote(FileNames.HatOpen);
            LoadNote(FileNames.Kick);
            LoadNote(FileNames.Ride);
            LoadNote(FileNames.RideBell);
            LoadNote(FileNames.Snare);
            LoadNote(FileNames.Tamb);
            LoadNote(FileNames.TomHigh);
            LoadNote(FileNames.TomMid);
            LoadNote(FileNames.TomLow);
        }

        private static int GetResourceDescriptor(string fileName)
        {
            return fileName switch
            {
                FileNames.ClickLo => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.clicklo,
                FileNames.Cowbell => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.cowbell,
                FileNames.CrashL => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.crashl,
                FileNames.CrashR => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.crashr,
                FileNames.HatClosed => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.hatclosed,
                FileNames.HatOpen => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.hatopen,
                FileNames.Kick => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.kick,
                FileNames.RideBell => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.ridebell,
                FileNames.Ride => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.rideedge,
                FileNames.Snare => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.snare,
                FileNames.Tamb => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.tamb,
                FileNames.TomHigh => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.tomhi,
                FileNames.TomLow => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.tomlow,
                FileNames.TomMid => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.tommid,
                _ => _Microsoft.Android.Resource.Designer.ResourceConstant.Raw.clickhi,
            };
        }

        private int GetSoundHandle(string fileName)
        {
            return _soundHandles[fileName];
        }

        private void Clear() 
        {
            foreach (var sh in _soundHandles)
            {
                _soundPool.Unload(sh.Value);
            }
            _soundHandles.Clear();
        }

        private void OnLoadCompleteListener(object sender, SoundPool.LoadCompleteEventArgs args) => _soundPool?.Play(args.SampleId, 1f, 1f, 1, 0, 1f);

        public void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }
    }
}
