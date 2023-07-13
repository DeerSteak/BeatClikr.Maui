﻿using BeatClikr.Maui.ViewModels;
using MetroLog;
using Microsoft.AppCenter.Crashes;
using Plugin.MauiMTAdmob;
using Plugin.MauiMTAdmob.Extra;
using System.Text;

namespace BeatClikr.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = ServiceHelper.GetService<Views.AppShell>();
    }

    protected override void OnStart()
    {
        base.OnStart();
        Crashes.GetErrorAttachments = (ErrorReport report) =>
        {
            var path = Path.Combine(
                FileSystem.CacheDirectory,
                "BeatClikrLog");
            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);

                return new ErrorAttachmentLog[]
                {
                ErrorAttachmentLog.AttachmentWithText(text, "error.log"),
                };
            }
            return new ErrorAttachmentLog[0];
        };

        if (!Preferences.ContainsKey(PreferenceKeys.InstantBeat))
            Preferences.Set(PreferenceKeys.InstantBeat, FileNames.ClickHi);

        if (!Preferences.ContainsKey(PreferenceKeys.InstantRhythm))
            Preferences.Set(PreferenceKeys.InstantRhythm, FileNames.ClickLo);

        if (!Preferences.ContainsKey(PreferenceKeys.RehearsalBeat))
            Preferences.Set(PreferenceKeys.RehearsalBeat, FileNames.ClickHi);

        if (!Preferences.ContainsKey(PreferenceKeys.RehearsalRhythm))
            Preferences.Set(PreferenceKeys.RehearsalRhythm, FileNames.ClickLo);

        if (!Preferences.ContainsKey(PreferenceKeys.LiveBeat))
            Preferences.Set(PreferenceKeys.LiveBeat, FileNames.ClickHi);

        if (!Preferences.ContainsKey(PreferenceKeys.LiveRhythm))
            Preferences.Set(PreferenceKeys.LiveRhythm, FileNames.ClickLo);

    }

    protected override void OnSleep()
    {
        var deviceDisplay = ServiceHelper.GetService<IDeviceDisplay>();
        if (deviceDisplay != null)
            deviceDisplay.KeepScreenOn = false;
        var metronome = ServiceHelper.GetService<MetronomeClickerViewModel>();
        metronome?.StopCommand.Execute(null);
        base.OnSleep();
    }
}

