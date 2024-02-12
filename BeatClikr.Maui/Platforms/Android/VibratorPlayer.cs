using Android.Content;
using Android.OS;

namespace BeatClikr.Maui.Platforms.Droid
{
    public class VibratorPlayer
    {
        private Vibrator _vibrator;
        public const float MAXIMUM_VIBRATION_SCALING = 3.0f;

        private VibrationEffect _beatEffect;
        private VibrationEffect _rhythmEffect;
        private bool _useHaptic;
        private bool _canVibrate;

        public VibratorPlayer()
        {
            _vibrator = GetVibrator();
            _canVibrate = HasHardwareSupport();
        }

        public static Vibrator GetVibrator()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                var mgr = MainApplication.Context.GetSystemService(Context.VibratorManagerService) as VibratorManager;
                return mgr?.DefaultVibrator;
            }
            else
                return MainApplication.Context.GetSystemService(Context.VibratorService) as Vibrator;
        }

        public bool HasHardwareSupport()
        {
            return _vibrator != null && _vibrator.HasVibrator;
        }

        public void SetHaptic()
        {
            _useHaptic = Preferences.Get(PreferenceKeys.UseHaptic, false);            
            _beatEffect = VibrationEffect.CreateOneShot(10, 255);
            _rhythmEffect = VibrationEffect.CreateOneShot(10, 128);
            
        }

        public void PlayBeat()
        {        
            if (_useHaptic && _canVibrate)
                _vibrator?.Vibrate(_beatEffect);                        
        }

        public void PlayRhythm()
        {
            if (_useHaptic && _canVibrate)
                _vibrator?.Vibrate(_rhythmEffect); ;
        }
    }


}
