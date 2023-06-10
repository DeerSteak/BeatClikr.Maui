using Plugin.LocalNotification;

namespace BeatClikr.Maui.Helpers
{
    public static class LocalNotificationsHelper
	{
        public static int REMINDER_ID = 1000;

		public static async Task<bool> RegisterForNotifications()
		{
			var success = false;
            var center = ServiceHelper.GetService<INotificationService>();
            var perm = await center.AreNotificationsEnabled();

            if (!perm)
                perm = await center.RequestNotificationPermission();

            if (!perm)
                return perm;
            else
            {
                var req = new NotificationRequest()
                {
                    NotificationId = REMINDER_ID,
                    BadgeNumber = 1,
                    Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions()
                    {
                        ChannelId = "beatClikrReminders"
                    },
                    iOS = new Plugin.LocalNotification.iOSOption.iOSOptions()
                    {
                        ApplyBadgeValue = true,
                        SummaryArgument = "It's time to practice!",
                    },
                    Schedule = new NotificationRequestSchedule()
                    {
                        NotifyTime = DateTime.Now.AddDays(1),
                        RepeatType = NotificationRepeat.Daily
                    }
                };

                await center.Show(req);
            }

            return success;
		}

        public static void ClearReminderNotifications()
        {
            var center = ServiceHelper.GetService<INotificationService>();
            center.Clear(REMINDER_ID);
        }
    }    
}