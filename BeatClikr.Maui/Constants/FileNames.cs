namespace BeatClikr.Maui.Constants;

public static class FileNames
{
    public const string Cowbell = "cowbell";
    public const string CrashL = "crashl";
    public const string CrashR = "crashr";
    public const string HatClosed = "hatclosed";
    public const string HatOpen = "hatopen";
    public const string Kick = "kick";
    public const string Ride = "ride";
    public const string RideBell = "ridebell";
    public const string Snare = "snare";
    public const string Tamb = "tamb";
    public const string TomHigh = "tomhi";
    public const string TomLow = "tomlow";
    public const string TomMid = "tommid";
    public const string Set1 = "set1";
    public const string Set2 = "set2";
    public static readonly string Platform = DeviceInfo.Platform == DevicePlatform.iOS ? "apple" : "other";
    public static readonly string Extension = DeviceInfo.Platform == DevicePlatform.iOS ? "caf" : "wav";
}
