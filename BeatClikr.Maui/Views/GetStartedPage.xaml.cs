using BeatClikr.Maui.Services.Interfaces;
using Microsoft.AppCenter.Analytics;

namespace BeatClikr.Maui.Views;

public partial class GetStartedPage : ContentPage
{
    public GetStartedPage(ViewModels.GetStartedViewModel getStartedViewModel)
    {
        InitializeComponent();
        BindingContext = getStartedViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.GetStartedViewModel).SetImageHeight();
        Analytics.TrackEvent($"{GetType()} appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Analytics.TrackEvent($"{GetType()} disappearing");
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(PreferenceKeys.Onboarded, DateTime.Now);
        await PermissionsHelper.AskAllPermissions();
        await Navigation.PopModalAsync();
    }
}
