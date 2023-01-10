namespace BeatClikr.Maui.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!Preferences.ContainsKey(PreferenceKeys.UsePersonalizedAds))
        {
            Preferences.Set(PreferenceKeys.UsePersonalizedAds, true);
            //do first time stuff
        }
    }
}
