namespace BeatClikr.Maui.CustomControls;

public partial class MetronomeClickerView : ContentView
{
    private int translateTo = 235;
    private uint beatLengthInMs = 500;
    public MetronomeClickerView(ViewModels.MetronomeClickerViewModel viewModel)
    {
        InitializeComponent();
        viewModel.Animate = this.Animate;
        viewModel.SetBeatMilliseconds = this.SetBeatMilliseconds;
        BindingContext = viewModel;
    }

    public MetronomeClickerView()
        : this(ServiceHelper.GetService<ViewModels.MetronomeClickerViewModel>())
    {
    }

    private void Animate()
    {
        BeatBox.TranslateTo(translateTo, 0, beatLengthInMs, Easing.Linear);
        translateTo = translateTo == 235 ? 0 : 235;
    }

    private void SetBeatMilliseconds(uint milliseconds)
    {
        beatLengthInMs = milliseconds;
    }
}
