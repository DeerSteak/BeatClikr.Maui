using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatClikr.Maui.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    [ObservableProperty]
    private string _versionInfo;

    private MetronomeClickerViewModel _metronomeClickerViewModel;

    public AppShellViewModel(IAppInfo appInfo, MetronomeClickerViewModel metronomeClickerViewModel)
    {
        VersionInfo = $"Version {appInfo.Version}";
        _metronomeClickerViewModel = metronomeClickerViewModel;
    }

    public void AskFlashlight()
    {

    }
}

