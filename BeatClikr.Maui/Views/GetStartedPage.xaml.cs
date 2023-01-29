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
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(PreferenceKeys.Onboarded, DateTime.Now);
        var page = ServiceHelper.GetService<AppShell>();
        await Navigation.PushAsync(page, true);
        Application.Current.MainPage = page;
    }
}
