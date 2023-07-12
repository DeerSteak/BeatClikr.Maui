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
            var success = false;
            var center = ServiceHelper.GetService<INotificationService>();

            var perm = await center.RequestNotificationPermission();

            if (!perm)
                return perm;
            else
            {
                var req = new NotificationRequest()
                {
                    NotificationId = REMINDER_ID,
                    BadgeNumber = 1,
                    Title = "BeatClikr Practice Reminder",
                    Description = "This notification means it's time to grab your instrument and get practicing!",
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
                        NotifyTime = DateTime.Now,
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

        private void DoSnackbar(bool isRegistered)
        {
            var msg = isRegistered
                ? "You will receive reminders daily, starting this time tomorrow"
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

