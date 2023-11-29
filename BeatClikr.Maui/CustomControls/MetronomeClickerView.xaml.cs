namespace BeatClikr.Maui.CustomControls;

public partial class MetronomeClickerView : ContentView
{
    private const int TRANSLATION_MAX = 235;
    private int translateTo = TRANSLATION_MAX;
    private int translateFrom = 0;
    private uint beatLengthInMs = 500;
    
    public MetronomeClickerView(ViewModels.MetronomeClickerViewModel viewModel)
    {
        InitializeComponent();
        viewModel.Animate = this.Animate;
        viewModel.SetBeatMilliseconds = this.SetBeatMilliseconds;
        viewModel.ResetAnimator = ResetAnimator;
        BindingContext = viewModel;
    }

    public MetronomeClickerView()
        : this(ServiceHelper.GetService<ViewModels.MetronomeClickerViewModel>())
    {
    }

    private void Animate()
    {
        BeatBox.CancelAnimations();
        BeatBox.TranslationX = translateFrom;
        BeatBox.TranslateTo(translateTo, 0, beatLengthInMs, Easing.Linear);
        translateTo = translateTo == TRANSLATION_MAX ? 0 : TRANSLATION_MAX;
        translateFrom = translateTo == TRANSLATION_MAX ? 0 : TRANSLATION_MAX;
    }

    private void SetBeatMilliseconds(uint milliseconds)
    {
        beatLengthInMs = milliseconds;
    }

    private void ResetAnimator()
    {
        translateTo = TRANSLATION_MAX;
        translateFrom = 0;
        BeatBox.CancelAnimations();
        BeatBox.TranslationX = 0;
    }
}
