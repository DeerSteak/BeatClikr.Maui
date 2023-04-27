namespace BeatClikr.Maui.CustomControls;

public partial class MetronomeClickerView : ContentView
{
    private int translateTo = 235;
    private int translateFrom = 0;
    private uint beatLengthInMs = 500;
    private const int TRANSLATION_MAX = 235;
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
        BeatBox.TranslationX = translateFrom;
        BeatBox.TranslateTo(translateTo, 0, beatLengthInMs, Easing.Linear);
        translateTo = translateTo == TRANSLATION_MAX ? 0 : TRANSLATION_MAX;
        translateFrom = translateTo == TRANSLATION_MAX ? 0 : TRANSLATION_MAX;
    }

    private void SetBeatMilliseconds(uint milliseconds)
    {
        beatLengthInMs = milliseconds;
    }
}
