using System;
namespace BeatClikr.Maui.Services.Interfaces
{
	public interface IAdTrackingHandlerService
	{
		Task<bool> AskTrackingPermission();
		Task GoToTrackingSettings();
	}
}

