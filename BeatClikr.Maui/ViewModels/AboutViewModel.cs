using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeatClikr.Maui.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    private IShellService _pageService;
    public AboutViewModel(IShellService pageService)
    {
        _pageService = pageService;
    }

    [RelayCommand]
    private async void IconAuthorLabel() => await Browser.OpenAsync("https://www.flaticon.com/authors/freepik");

    [RelayCommand]
    private async void BulbAuthorLabel() => await Browser.OpenAsync("https://www.flaticon.com/authors/good-ware");

    [RelayCommand]
    private async void SiteLabel() => await Browser.OpenAsync("https://www.flaticon.com/");

    [RelayCommand]
    private async void SendFeedback() => await SendEmail();

    private async Task SendEmail()
    {
        string subject = "BeatClikr feedback";
        string body = $"BeatClikr Version: {VersionTracking.CurrentVersion}\n\r\n\r";
        List<string> recipients = new List<string>
        {
            "beatclikr@gmail.com"
        };
        try
        {
            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                To = recipients,
            };
            await Email.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException)
        {
            await _pageService.DisplayAlert("Email not supported", "Email is not supported or correctly configured on this device. You can still contact us at beatclikr@gmail.com.", "OK");
        }
        catch (Exception)
        {
            await _pageService.DisplayAlert("Error sending email", "There was an error setting up email. You can still contact us at beatclikr@gmail.com.", "OK");
        }
    }
}

