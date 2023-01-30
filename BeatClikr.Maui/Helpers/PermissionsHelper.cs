using System;
using BeatClikr.Maui.Services.Interfaces;

namespace BeatClikr.Maui.Helpers
{
	public static class PermissionsHelper
	{
		public static async Task SetupFlashlight()
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

        public static async Task FirstTimeFlashlightQuestion()
        {
            if (!Preferences.ContainsKey(PreferenceKeys.UseFlashlight))
            {
                var baseText = "BeatClikr can show the beat using your device's flashlight. Do you want to use that feature?";
                var androidText = baseText + " You will be asked permission to use your device's camera.";
                var questionText = DeviceInfo.Platform == DevicePlatform.iOS ? baseText : androidText;
                var questionResponse = await (ServiceHelper.GetService<INonShellNavProvider>()).DisplayAlert("Use Flashlight?", questionText, "Yes", "No");
                Preferences.Set(PreferenceKeys.UseFlashlight, questionResponse);
            }

            await SetupFlashlight();
        }
    }    
}

