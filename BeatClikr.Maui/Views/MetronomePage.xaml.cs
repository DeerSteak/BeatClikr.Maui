namespace BeatClikr.Maui.Views;

public partial class MetronomePage : ContentPage
{
	public MetronomePage(ViewModels.MetronomeViewModel metronomeViewModel)
	{
		BindingContext = metronomeViewModel;
        InitializeComponent();
    }

    public MetronomePage() : this(ServiceHelper.GetService<ViewModels.MetronomeViewModel>())
	{
		
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.MetronomeViewModel).Init();
    }

    void Button_Clicked(object sender, EventArgs e)
    {
        (BindingContext as ViewModels.MetronomeViewModel).PlayStopToggledCommand.Execute(null);
    }
}
