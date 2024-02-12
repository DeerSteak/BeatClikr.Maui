﻿namespace BeatClikr.Maui.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage(ViewModels.AboutViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    public AboutPage() : this(IPlatformApplication.Current.Services.GetService<ViewModels.AboutViewModel>())
    {

    }
}
