using System;
namespace BeatClikr.Maui.Services.Interfaces
{
	public interface IPermissionService
	{
        Task SetupFlashlight();
        Task SetupHaptic();
        Task FirstTimeFlashlightQuestion();
        Task FirstTimeHapticQuestion();
        Task AskAllPermissions();
    }
}

