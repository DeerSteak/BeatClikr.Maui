using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.LocalNotification;

namespace BeatClikr.Maui.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        private const int REMINDER_ID = 1000;

        public async Task<bool> RegisterForNotifications()
        {
            var center = ServiceHelper.GetService<INotificationService>();
            var perm = await center.RequestNotificationPermission();

            bool success;
            if (!perm)
                return perm;
            else
            {
                int totalSeconds = (int)(DateTime.Now - DateTime.Today).TotalSeconds;
                int seconds = Preferences.Get(PreferenceKeys.ReminderTime, (int)totalSeconds);

                var req = new NotificationRequest()
                {
                    NotificationId = REMINDER_ID,
                    BadgeNumber = 1,
                    Title = "BeatClikr Practice Reminder",
                    Description = "It's time to grab your instrument and practice!",
                    Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions()
                    {
                        ChannelId = "beatClikrReminders"
                    },
                    iOS = new Plugin.LocalNotification.iOSOption.iOSOptions()
                    {
                        SummaryArgument = "It's time to practice!",
                        Priority = Plugin.LocalNotification.iOSOption.iOSPriority.TimeSensitive
                    },
                    Schedule = new NotificationRequestSchedule()
                    {
                        NotifyTime = DateTime.Today.AddSeconds(seconds),
                        RepeatType = NotificationRepeat.Daily
                    }
                };

                await center.Show(req);
                success = true;
            }

            DoSnackbar(success);

            return success;
        }

        public void ClearReminderNotifications()
        {
            var center = ServiceHelper.GetService<INotificationService>();
            center.Clear(REMINDER_ID);
            DoSnackbar(false);
        }

        private static void DoSnackbar(bool isRegistered)
        {
            var totalSeconds = (int)(DateTime.Now - DateTime.Today).TotalSeconds;
            var seconds = Preferences.Get(PreferenceKeys.ReminderTime, (int)totalSeconds);
            var timeSpan = TimeSpan.FromSeconds(seconds);
            var time = "";

            if (isRegistered)
                time = timeSpan.ToString(@"hh\:mm");

            var msg = isRegistered
                ? $"You will receive reminders daily at {time}"
            : "Previously-scheduled notifications canceled.";

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#408CC4"),
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Black,
                CornerRadius = new CornerRadius(5)
            };

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var snackBar = Snackbar.Make(msg, null, "OK", TimeSpan.FromSeconds(5), snackbarOptions);
                await snackBar.Show();
            });
        }
    }
}

