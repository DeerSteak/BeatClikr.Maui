using System;
namespace BeatClikr.Maui.Services.Interfaces
{
	public interface INonShellNavProvider
	{
		Task DisplayAlert(string title, string message, string cancel);
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
    }
}

