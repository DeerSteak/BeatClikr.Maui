using Android.App;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;

namespace BeatClikr.Maui;

[Activity(Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges =
        ConfigChanges.ScreenSize
        | ConfigChanges.Orientation
        | ConfigChanges.UiMode
        | ConfigChanges.ScreenLayout
        | ConfigChanges.SmallestScreenSize
        | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        MobileAds.Initialize(this);
        if (OperatingSystem.IsAndroidVersionAtLeast(8))
        {
            SetupNotificationChannels();
        }
    }

    private void SetupNotificationChannels()
    {
        var reminderChannelId = "beatClikrReminders";
        var reminderChannelName = "Practice Reminders";
        var reminderChannelDescription = "Daily practice reminder notifications to get you to pick up your instrument (set up in the app)";
        var defaultImportance = Android.App.NotificationImportance.Default;
        var reminderChannel = new NotificationChannel(reminderChannelId, reminderChannelName, defaultImportance);
        reminderChannel.Description = reminderChannelDescription;

        var notificationManager = GetSystemService(NotificationService) as NotificationManager;
        if (notificationManager != null)
            notificationManager.CreateNotificationChannel(reminderChannel);
    }
}

