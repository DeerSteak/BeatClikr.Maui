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

    void ListView_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
    {
		(BindingContext as ViewModels.LibraryViewModel).SelectionChangedCommand.Execute(e.SelectedItemIndex);
    }
}
