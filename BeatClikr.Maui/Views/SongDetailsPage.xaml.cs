using Microsoft.AppCenter.Analytics;
using System.Runtime.CompilerServices;

namespace BeatClikr.Maui.Views;

public partial class SongDetailsPage : ContentPage
{
    public SongDetailsPage(ViewModels.SongDetailsViewModel songDetailsViewModel)
    {
        InitializeComponent();
        BindingContext = songDetailsViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AnalyticsHelper.TrackEvent($"{GetType()} appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        AnalyticsHelper.TrackEvent($"{GetType()} disappearing");
    }
}
