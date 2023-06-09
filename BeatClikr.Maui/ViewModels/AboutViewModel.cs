using BeatClikr.Maui.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace BeatClikr.Maui.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    [ObservableProperty]
    private string _year;

    private IAppInfo _appInfo;
    private IShellService _shellService;
    private ILogger<AboutViewModel> _logger;

    public AboutViewModel(IShellService shellService, IAppInfo appInfo, ILogger<AboutViewModel> logger)
    {
        _shellService = shellService;
        _appInfo = appInfo;
        _logger = logger;

        Year = $"\u00a9{DateTime.Now.Year} Ben Funk";
    }

    [RelayCommand]
    private async void SendFeedback() => await SendEmail();

    private async Task SendEmail()
    {
        string subject = "BeatClikr feedback";
        string body = $"BeatClikr Version: {_appInfo.Version.ToString()}\n\r\n\r";
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
        catch (FeatureNotSupportedException ex)
        {
            await _shellService.DisplayAlert("Email not supported", "Email is not supported or correctly configured on this device. You can still contact us at beatclikr@gmail.com.", "OK");
            _logger.LogError("Email not supported exception: ", ex);
        }
        catch (Exception ex)
        {
            await _shellService.DisplayAlert("Error sending email", "There was an error setting up email. You can still contact us at beatclikr@gmail.com.", "OK");
            _logger.LogError("Error: ", ex);
        }
    }
}

