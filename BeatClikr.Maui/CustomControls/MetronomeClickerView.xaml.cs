namespace BeatClikr.Maui.CustomControls;

public partial class MetronomeClickerView : ContentView
{
    public MetronomeClickerView(ViewModels.MetronomeClickerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public MetronomeClickerView()
        : this(ServiceHelper.GetService<ViewModels.MetronomeClickerViewModel>())
    {
    }
}
