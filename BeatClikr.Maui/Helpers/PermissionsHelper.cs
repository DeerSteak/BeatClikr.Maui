using System;
using BeatClikr.Maui.Services.Interfaces;

namespace BeatClikr.Maui.Helpers
{
	public static class PermissionsHelper
	{
		private static async Task SetupFlashlight()
		{
            var result = await Permissions.CheckStatusAsync<Permissions.Flashlight>();
            if (result == PermissionStatus.Granted)
                return;

            var useFlashlight = Preferences.Get(PreferenceKeys.UseFlashlight, false);

            if (!useFlashlight)
                return;

            result = await Permissions.RequestAsync<Permissions.Flashlight>();
            if (result != PermissionStatus.Granted)
            {
                await (ServiceHelper.GetService<INonShellNavProvider>()).DisplayAlert("Flashlight Permission Denied", "BeatClikr will not use the flashlight. If you change your mind, you can enable the flashlight again on the Settings page.", "OK");
            }

            var pref = result == PermissionStatus.Granted;
            Preferences.Set(PreferenceKeys.UseFlashlight, pref);
        }

        private static async Task SetupHaptic()
        {
            var result = await Permissions.CheckStatusAsync<Permissions.Vibrate>();
            if (result == PermissionStatus.Granted)
                return;

            var useHaptic = Preferences.Get(PreferenceKeys.UseHaptic, false);

            if (!useHaptic)
                return;

            result = await Permissions.RequestAsync<Permissions.Vibrate>();
            if (result != PermissionStatus.Granted)
            {
                await (ServiceHelper.GetService<INonShellNavProvider>()).DisplayAlert("Vibration Permission Denied", "BeatClikr will vibrate. If you change your mind, you can enable the vibration setting again on the Settings page.", "OK");
            }

            var pref = result == PermissionStatus.Granted;
            Preferences.Set(PreferenceKeys.UseHaptic, pref);
        }

        private static async Task FirstTimeFlashlightQuestion()
        {
            if (!Preferences.ContainsKey(PreferenceKeys.UseFlashlight))
            {
                var baseText = "BeatClikr can show the beat using your device's flashlight. Do you want to use that feature?";
                var androidText = baseText + " You will be asked permission to use your device's camera.";
                var questionText = DeviceInfo.Platform == DevicePlatform.iOS ? baseText : androidText;
                var questionResponse = await (ServiceHelper.GetService<INonShellNavProvider>()).DisplayAlert("Use Flashlight?", questionText, "Yes", "No");
                Preferences.Set(PreferenceKeys.UseFlashlight, questionResponse);
            }
            Preferences.Set(PreferenceKeys.HasAskedFlashlight, true);
            await SetupFlashlight();
        }

        private static async Task FirstTimeHapticQuestion()
        {
            if (!Preferences.ContainsKey(PreferenceKeys.UseHaptic))
            {
                var baseText = "BeatClikr can vibrate the device to indicate the beat. Do you want to use that feature?";
                var androidText = baseText + " You will be asked permission to use your device's camera.";
                var questionText = DeviceInfo.Platform == DevicePlatform.iOS ? baseText : androidText;
                var questionResponse = await (ServiceHelper.GetService<INonShellNavProvider>()).DisplayAlert("Use Vibration?", questionText, "Yes", "No");
                Preferences.Set(PreferenceKeys.UseHaptic, questionResponse);
            }
            Preferences.Set(PreferenceKeys.HasAskedHaptic, true);
            await SetupHaptic();
        }

        public static async Task AskAllPermissions()
        {
            if (!Preferences.ContainsKey(PreferenceKeys.HasAskedFlashlight))
            {
                await FirstTimeFlashlightQuestion().ConfigureAwait(true);
            }
            if (!Preferences.ContainsKey(PreferenceKeys.HasAskedHaptic))
            {
                await FirstTimeHapticQuestion().ConfigureAwait(true);
            }
        }
    }    
}

