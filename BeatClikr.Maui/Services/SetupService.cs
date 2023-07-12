using BeatClikr.Maui.Services.Interfaces;

namespace BeatClikr.Maui.Helpers
{
    public class SetupService : ISetupService
    {
        private async Task SetupFlashlight()
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

        private async Task SetupHaptic()
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

        private async Task SetupReminders()
        {
            ILocalNotificationService service = ServiceHelper.GetService<ILocalNotificationService>();
            if (service != null)
            {
                await service.RegisterForNotifications();
            }
        }

        private async Task FirstTimeFlashlightQuestion()
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

        private async Task FirstTimeHapticQuestion()
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

        private async Task FirstTimePracticeReminders()
        {
            if (!Preferences.ContainsKey(PreferenceKeys.PracticeReminders))
            {
                var questionText = "BeatClikr can remind you to practice at this time each day. Would you like these remidners? If so, press Yes and select Allow on the next prompt.";
                var questionResponse = await (ServiceHelper.GetService<INonShellNavProvider>()).DisplayAlert("Get practice reminders?", questionText, "Yes", "No");
                Preferences.Set(PreferenceKeys.PracticeReminders, questionResponse);
            }
            Preferences.Set(PreferenceKeys.HasAskedReminders, true);
            await SetupReminders();
        }

        public async Task SetupFeatures()
        {
            if (!Preferences.ContainsKey(PreferenceKeys.HasAskedFlashlight))
                await FirstTimeFlashlightQuestion().ConfigureAwait(true);
            if (!Preferences.ContainsKey(PreferenceKeys.HasAskedHaptic))
                await FirstTimeHapticQuestion().ConfigureAwait(true);
            if (!Preferences.ContainsKey(PreferenceKeys.HasAskedReminders))
                await FirstTimePracticeReminders().ConfigureAwait(true);
        }
    }
}

