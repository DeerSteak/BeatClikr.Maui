using BeatClikr.Maui.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatClikr.Maui.ViewModels
{
    public partial class GetStartedViewModel : ObservableObject
    {
        [ObservableProperty]
        private IDeviceDisplay _deviceDisplay;

        [ObservableProperty]
        private int _imageHeight;

        [ObservableProperty]
        private List<CarouselViewItem> _carouselViewItems;

        public GetStartedViewModel(IDeviceDisplay deviceDisplay)
        {
            _deviceDisplay = deviceDisplay;
            _deviceDisplay.MainDisplayInfoChanged += (s, e) => SetImageHeight();
            CarouselViewItems = new List<CarouselViewItem>
            {
                new CarouselViewItem { ImageName = "screen1", Description = "Stuff about the main page, how to access the menu"},
                new CarouselViewItem { ImageName = "screen2", Description = "Stuff about the menu"}
            };
            SetImageHeight();
        }

        public void SetImageHeight()
        {
            ImageHeight = (int)((_deviceDisplay.MainDisplayInfo.Height / _deviceDisplay.MainDisplayInfo.Density) * 0.67);
        }
    }
}
