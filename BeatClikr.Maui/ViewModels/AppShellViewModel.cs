using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BeatClikr.Maui.ViewModels
{
	public partial class AppShellViewModel : ObservableObject
	{
		[ObservableProperty]
		private string _versionInfo;

		public AppShellViewModel(IDeviceInfo deviceInfo)
		{
			VersionInfo = $"Version {deviceInfo.VersionString}";
		}
	}
}

