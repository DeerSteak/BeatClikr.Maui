namespace BeatClikr.Maui.Views;

public partial class LibraryPage : ContentPage
{
    public LibraryPage(ViewModels.LibraryViewModel libraryViewModel)
    {
        InitializeComponent();
        BindingContext = libraryViewModel;
    }

    public LibraryPage() : this(ServiceHelper.GetService<ViewModels.LibraryViewModel>())
    {

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.LibraryViewModel).Init();
    }
}
