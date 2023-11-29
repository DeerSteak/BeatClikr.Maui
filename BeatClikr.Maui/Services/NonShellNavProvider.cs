using BeatClikr.Maui.Services.Interfaces;

namespace BeatClikr.Maui.Services
{
    public class NonShellNavProvider : INonShellNavProvider
	{
        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);           
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        public async Task<Page> PopModalAsync()
        {
            return await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}

