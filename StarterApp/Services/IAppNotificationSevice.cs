namespace StarterApp.Services;

public interface IAppNotificationService
{
    Task ShowAsync(string title, string message);
}
