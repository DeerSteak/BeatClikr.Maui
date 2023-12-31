﻿using Microsoft.Extensions.Logging;
using Plugin.StoreReview.Abstractions;

namespace BeatClikr.Maui.Views;

public partial class InstantMetronomePage : ContentPage
{
    private ILogger _logger;
    private IStoreReview _review;
    public InstantMetronomePage(
            ViewModels.InstantMetronomeViewModel metronomeViewModel,
            ILogger<InstantMetronomePage> logger,
            IStoreReview review
        )
    {
        BindingContext = metronomeViewModel;
        _logger = logger;
        _review = review;
        InitializeComponent();
        AdView.AdsId = DeviceInfo.Platform == DevicePlatform.Android
            ? "ca-app-pub-8377432895177958/9298625858"
            : "ca-app-pub-8377432895177958/7490720167";
    }

    public InstantMetronomePage() : this(
            ServiceHelper.GetService<ViewModels.InstantMetronomeViewModel>(),
            ServiceHelper.GetService<ILogger<InstantMetronomePage>>(),
            ServiceHelper.GetService<IStoreReview>()
        )
    { }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ViewModels.InstantMetronomeViewModel).Init();
        var istest =
#if DEBUG
            true;
#else
            false;
#endif
        _review.RequestReview(istest);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        var vm = BindingContext as ViewModels.InstantMetronomeViewModel;
        if (vm.WasPlaying)
            vm.StopCommand.Execute(null);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var onboarded = Preferences.Get(PreferenceKeys.Onboarded,
            new DateTime(1900, 1, 1));
        if (onboarded < new DateTime(2023, 1, 29))
            Shell.Current.Navigation.PushModalAsync(ServiceHelper.GetService<GetStartedPage>());
    }
}
