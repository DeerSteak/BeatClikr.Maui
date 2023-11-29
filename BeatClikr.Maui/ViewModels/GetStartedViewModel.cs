using BeatClikr.Maui.Models;
using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels
{
    public partial class GetStartedViewModel : ObservableObject
    {
        private IDeviceDisplay _deviceDisplay;
        private ISetupService _permissionService;
        private INonShellNavProvider _nonShellNavProvider;

        [ObservableProperty]
        private int _imageHeight;

        [ObservableProperty]
        private List<CarouselViewItem> _carouselViewItems;

        [RelayCommand]
        private async Task GetStarted()
        {
            Preferences.Set(PreferenceKeys.Onboarded, DateTime.Now);
            await _permissionService.SetupFeatures();
            await _nonShellNavProvider.PopModalAsync();
        }


        public GetStartedViewModel(IDeviceDisplay deviceDisplay, IShellService shellService, ISetupService permissionService, INonShellNavProvider nonShellNavProvider)
        {
            _deviceDisplay = deviceDisplay;
            _deviceDisplay.MainDisplayInfoChanged += (s, e) => SetImageHeight();
            _permissionService = permissionService;
            _nonShellNavProvider = nonShellNavProvider;
            CarouselViewItems = new List<CarouselViewItem>
            {
                new CarouselViewItem { ImageName = "screen1", Description = "The Instant Metronome is a quick and easy way to set BPM and Groove and just use a metronome. The menu in the upper-right is for other features."},
                new CarouselViewItem { ImageName = "screen2", Description = "On the menu, you can build a song collection on the Library page. Two playlists, Rehearsal and Live, let you add songs in any order you want. Rehearsal metronome sound is always on. Live sound clicks off but then just animates the icon while you play."}
            };
            SetImageHeight();
        }

        public void SetImageHeight()
        {
            ImageHeight = (int)(_deviceDisplay.MainDisplayInfo.Height / _deviceDisplay.MainDisplayInfo.Density * 0.75);
            foreach (var item in CarouselViewItems)
                item.HeightRequest = (int)((double)ImageHeight * 0.67);
        }
    }
}
