﻿namespace BeatClikr.Maui.Views;

public partial class HelpPage : ContentPage
{
    public HelpPage(ViewModels.HelpViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public HelpPage() : this(IPlatformApplication.Current.Services.GetService<ViewModels.HelpViewModel>())
    {

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}
