using BeatClikr.Maui.Services.Interfaces;

namespace BeatClikr.Maui.Services;

public class ShellService : IShellService
{
    private Shell _appShell = Shell.Current;

    public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel) => await _appShell.DisplayAlert(title, message, accept, cancel);
    public async Task DisplayAlert(string title, string message, string cancel) => await _appShell.DisplayAlert(title, message, cancel);

    public async Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons) => await _appShell.DisplayActionSheet(title, cancel, destruction, buttons);

    public async Task PushModalAsync(Page page) => await _appShell.Navigation.PushModalAsync(page);
    public async Task<Page> PopModalAsync(bool animated = true) => await _appShell.Navigation.PopModalAsync(animated);
    public async Task PushAsync(Page page) => await _appShell.Navigation.PushAsync(page);
    public async Task PushAsync(Page page, bool animated) => await _appShell.Navigation.PushAsync(page, animated);
    public async Task<Page> PopAsync() => await _appShell.Navigation.PopAsync();


    public async Task GoToAsync(ShellNavigationState state) => await _appShell.GoToAsync(state);
    public async Task GoToAsync(ShellNavigationState state, bool animate) => await _appShell.GoToAsync(state, animate);
    public async Task GoToAsync(ShellNavigationState state, IDictionary<string, object> parameters) => await _appShell.GoToAsync(state, parameters);
    public async Task GoToAsync(ShellNavigationState state, bool animate, IDictionary<string, object> parameters) => await _appShell.GoToAsync(state, animate, parameters);
}

