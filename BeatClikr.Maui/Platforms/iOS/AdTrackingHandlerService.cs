using AppTrackingTransparency;
using BeatClikr.Maui.Services.Interfaces;
namespace BeatClikr.Maui.Platforms.iOS
{
    public class AdTrackingHandlerService : IAdTrackingHandlerService
    {
        private IShellService _shellService;
        private IAppInfo _appInfo;
        public AdTrackingHandlerService(IShellService shellService, IAppInfo appInfo)
        {
            _shellService = shellService;
            _appInfo = appInfo;
        }

        public async Task<bool> AskTrackingPermission()
        {
            if (!OperatingSystem.IsIOSVersionAtLeast(14))
            {
                return true;
            }

            var status = ATTrackingManager.TrackingAuthorizationStatus;
            switch (status)
            {
                case ATTrackingManagerAuthorizationStatus.Authorized:
                    return true;
                case ATTrackingManagerAuthorizationStatus.NotDetermined:
                    status = await ATTrackingManager.RequestTrackingAuthorizationAsync();
                    var trackingAllowed = status == ATTrackingManagerAuthorizationStatus.Authorized;
                    return trackingAllowed;
                default: //restricted or denied
                    return false;
            }
        }

        public async Task GoToTrackingSettings()
        {
            var goToSettings = await _shellService.DisplayAlert("Permission Denied",
                        "To allow tracking, you need to enable tracking in the device's settings.",
                        "OK",
                        "Cancel");
            if (goToSettings)
                _appInfo.ShowSettingsUI();
        }
    }
}

