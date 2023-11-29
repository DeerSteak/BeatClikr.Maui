namespace BeatClikr.Maui.CustomControls;

public partial class TitleView : ContentView
{
    public static BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(TitleView),
        "The Live Metronome",
        BindingMode.TwoWay,
        null,
        OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((TitleView)bindable).TitleLabel.Text = newValue.ToString();
    }

    public TitleView()
    {
        InitializeComponent();
    }
}
