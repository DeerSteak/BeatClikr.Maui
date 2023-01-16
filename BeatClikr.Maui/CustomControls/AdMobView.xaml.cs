namespace BeatClikr.Maui.CustomControls;

public partial class AdMobView : ContentView
{
	public AdMobView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty AdUnitIdProperty = BindableProperty.Create(
               nameof(AdUnitId),
               typeof(string),
               typeof(AdMobView),
               string.Empty
        , propertyChanged: AdsIdChanged);

    public string AdUnitId
    {
        get => (string)GetValue(AdUnitIdProperty);
        set => SetValue(AdUnitIdProperty, value);
    }

    private static void AdsIdChanged(BindableObject bindable, object oldVal, object newVal)
    {
        var ad = bindable as AdMobView;
        ad.AdUnitId = newVal.ToString();
    }
}
