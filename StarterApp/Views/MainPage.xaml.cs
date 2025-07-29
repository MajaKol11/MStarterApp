using StarterApp.ViewModels;
using System.Diagnostics;                 // ← Debug.WriteLine
using Microsoft.Maui.ApplicationModel;    // ← Permissions.*

namespace StarterApp.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                Debug.WriteLine($"[NotifPerm] POST_NOTIFICATIONS → {status}");
            }
        }
#endif
    }
}
