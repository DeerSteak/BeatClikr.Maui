namespace BeatClikr.Maui.CustomControls;

public partial class MetronomeClickerView : ContentView
{
	public MetronomeClickerView(MetronomeClickerViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	public MetronomeClickerView()
		: this(ServiceHelper.GetService<MetronomeClickerViewModel>())
	{		
	}
}
