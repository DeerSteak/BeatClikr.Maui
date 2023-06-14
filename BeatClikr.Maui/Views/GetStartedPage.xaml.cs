using BeatClikr.Maui.Services.Interfaces;

namespace BeatClikr.Maui.Views;

public partial class GetStartedPage : ContentPage
{
    private IPermissionService _permissionService;
    public GetStartedPage(ViewModels.GetStartedViewModel getStartedViewModel, IPermissionService permissionService)
    {
        InitializeComponent();
        BindingContext = getStartedViewModel;
        _permissionService = permissionService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.GetStartedViewModel).SetImageHeight();
        AnalyticsHelper.TrackEvent($"{GetType()} appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        AnalyticsHelper.TrackEvent($"{GetType()} disappearing");
    }
}
