﻿namespace BeatClikr.Maui.Views;

public partial class MetronomePage : ContentPage
{
	public MetronomePage(ViewModels.MetronomeViewModel metronomeViewModel)
	{
		InitializeComponent();
		BindingContext = metronomeViewModel;
	}
}
