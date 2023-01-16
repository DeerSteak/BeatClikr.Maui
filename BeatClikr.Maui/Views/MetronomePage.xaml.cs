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

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        (BindingContext as ViewModels.MetronomeViewModel).PlayStopToggledCommand.Execute(null);
    }
}
