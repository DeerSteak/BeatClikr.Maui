using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatClikr.Maui.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    [ObservableProperty]
    private string _versionInfo;

    private MetronomeClickerViewModel _metronomeClickerViewModel;

    public AppShellViewModel(IAppInfo appInfo)
    {
        VersionInfo = $"Version {appInfo.Version}";
    }

    public void AskFlashlight()
    {

    }
}

