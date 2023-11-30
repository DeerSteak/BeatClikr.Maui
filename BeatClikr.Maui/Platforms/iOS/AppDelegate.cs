using AppTrackingTransparency;
using Foundation;
using Google.MobileAds;
using StoreKit;
using UIKit;

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

        UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

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

