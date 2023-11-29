namespace BeatClikr.Maui.CustomControls;

public partial class SongCollectionItemView : ContentView
{
    public static BindableProperty TitleProperty = BindableProperty.Create(
        propertyName: nameof(Title),
        returnType: typeof(string),
        declaringType: typeof(SongCollectionItemView),
        defaultValue: "The Live Metronome",
        defaultBindingMode: BindingMode.TwoWay,
        validateValue: null,
        propertyChanged: OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((SongCollectionItemView)bindable).TitleLabel.Text = newValue.ToString();
    }

    public static BindableProperty ArtistProperty = BindableProperty.Create(
        nameof(Artist),
        typeof(string),
        typeof(SongCollectionItemView),
        "Ben Funk",
        BindingMode.TwoWay,
        null,
        OnArtistChanged);

    public string Artist
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    static void OnArtistChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((SongCollectionItemView)bindable).ArtistLabel.Text = newValue.ToString();
    }

    public static BindableProperty BpmProperty = BindableProperty.Create(
        nameof(Bpm),
        typeof(string),
        typeof(SongCollectionItemView),
        "Ben Funk",
        BindingMode.TwoWay,
        null,
        OnBpmChanged);

    public string Bpm
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    static void OnBpmChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((SongCollectionItemView)bindable).BpmLabel.Text = newValue.ToString();
    }

    public SongCollectionItemView()
    {
        InitializeComponent();
    }
}