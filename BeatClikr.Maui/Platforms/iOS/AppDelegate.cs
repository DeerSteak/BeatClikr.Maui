﻿using AppTrackingTransparency;
using Foundation;
using Google.MobileAds;
using UIKit;

namespace BeatClikr.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        var testId = "F8BB1C28-BAE8-11D6-9C31-00039315CD46";
        
        MobileAds.SharedInstance.RequestConfiguration.TestDeviceIdentifiers = new string[] { testId };

        MobileAds.SharedInstance.Start(AdsCompletionHandler);

        return base.FinishedLaunching(application, launchOptions);
    }

    private void AdsCompletionHandler(InitializationStatus status)
    {
        var trackerService = ServiceHelper.GetService<Services.Interfaces.IAdTrackingHandlerService>();
        Task.Run(async () => await trackerService?.AskTrackingPermission());
    }
}

