using System;
namespace BeatClikr.Maui.Services.Interfaces
{
	public interface IShellService
	{
		Task GoToAsync(ShellNavigationState state);
		Task GoToAsync(ShellNavigationState state, bool animate);
		Task GoToAsync(ShellNavigationState state, IDictionary<string, object> parameters);
		Task GoToAsync(ShellNavigationState state, bool animate, IDictionary<string, object> parameters);
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
		Task DisplayAlert(string title, string message, string cancel);
		Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);
		Task PushModalAsync(Page page);
		Task<Page> PopModalAsync(bool animated = true);
    }
}

