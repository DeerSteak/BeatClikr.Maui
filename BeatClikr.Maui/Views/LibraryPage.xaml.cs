﻿namespace BeatClikr.Maui.Views;

public partial class LibraryPage : ContentPage
{
    public LibraryPage(ViewModels.LibraryViewModel libraryViewModel)
    {
        InitializeComponent();
        AdView.AdsId = DeviceInfo.Platform == DevicePlatform.Android ? "ca-app-pub-8377432895177958/8340767404" : "ca-app-pub-8377432895177958/2324941827";
        BindingContext = libraryViewModel;
    }

    public LibraryPage() : this(ServiceHelper.GetService<ViewModels.LibraryViewModel>())
    {

    }
}
