using Microsoft.AppCenter.Analytics;

namespace BeatClikr.Maui.Views;

public partial class HelpPage : ContentPage
{
    public HelpPage(ViewModels.HelpViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public HelpPage() : this(ServiceHelper.GetService<ViewModels.HelpViewModel>())
    {

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Analytics.TrackEvent($"{GetType()} appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Analytics.TrackEvent($"{GetType()} disappearing");
    }
}
