using AppTrackingTransparency;
using Foundation;
using Google.MobileAds;
using StoreKit;
using UIKit;
using UserNotifications;

namespace BeatClikr.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        if (OperatingSystem.IsIOSVersionAtLeast(14))
        {
            ATTrackingManagerAuthorizationStatus status = ATTrackingManager.TrackingAuthorizationStatus;
            if (status == ATTrackingManagerAuthorizationStatus.NotDetermined || status == ATTrackingManagerAuthorizationStatus.Restricted)
            {
                ATTrackingManager.RequestTrackingAuthorization(TrackingCompletionHandler);
            }
            else
            {
                TrackingCompletionHandler(status);
            }
        }

        if (OperatingSystem.IsIOSVersionAtLeast(16) || OperatingSystem.IsMacCatalystVersionAtLeast(16)) 
        {
            UNUserNotificationCenter.Current.SetBadgeCount(0, null);
        } 
        else 
        {
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }        

        return MauiProgram.CreateMauiApp();
    }



    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        MobileAds.SharedInstance.Start(CompletionHandler);
        return base.FinishedLaunching(application, launchOptions);
    }

    private void CompletionHandler(InitializationStatus status) { }

    private void TrackingCompletionHandler(ATTrackingManagerAuthorizationStatus status)
    {
        
    }
}

