using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatClikr.Maui.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    [ObservableProperty]
    private string _versionInfo;

    private MetronomeClickerViewModel _metronomeClickerViewModel;

    public AppShellViewModel(IDeviceInfo deviceInfo, MetronomeClickerViewModel metronomeClickerViewModel)
    {
        VersionInfo = $"Version {deviceInfo.VersionString}";
        _metronomeClickerViewModel = metronomeClickerViewModel;
    }

    public void AskFlashlight()
    {

    }
}

