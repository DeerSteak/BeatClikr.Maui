using System;
namespace BeatClikr.Maui.Services.Interfaces
{
	public interface ILocalNotificationService
	{
        Task<bool> RegisterForNotifications();
        void ClearReminderNotifications();
    }
}

