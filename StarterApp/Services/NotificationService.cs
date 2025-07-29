using Plugin.LocalNotification;        
namespace StarterApp.Services;

public class NotificationService : IAppNotificationService
{
    private int _nextId = 1;

    public Task ShowAsync(string title, string message)
    {
        var request = new NotificationRequest
        {
            NotificationId = _nextId++,
            Title          = title,
            Description    = message
        };

        return LocalNotificationCenter.Current.Show(request);
      

    }
}
