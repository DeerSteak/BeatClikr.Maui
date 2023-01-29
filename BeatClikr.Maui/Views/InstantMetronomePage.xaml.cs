namespace BeatClikr.Maui.Views;

public partial class InstantMetronomePage : ContentPage
{
    public InstantMetronomePage(ViewModels.InstantMetronomeViewModel metronomeViewModel)
    {
        BindingContext = metronomeViewModel;
        InitializeComponent();
    }

    public InstantMetronomePage() : this(ServiceHelper.GetService<ViewModels.InstantMetronomeViewModel>())
    {

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.InstantMetronomeViewModel).Init();
    }

    void Button_Clicked(object sender, EventArgs e)
    {
        (BindingContext as ViewModels.InstantMetronomeViewModel).PlayStopToggledCommand.Execute(null);
    }
}
