using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using StarterApp.Database.Data;
using StarterApp.Services;
using StarterApp.ViewModels;
using StarterApp.Views;
using System.Diagnostics;
using Microsoft.Maui.ApplicationModel;   // Permissions.*

namespace StarterApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf",  "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseLocalNotification();                         // ← plugin initialisation

        /* ───────── Infrastructure ───────── */
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:DevelopmentConnection"]!));

        /* ───────── Core services ────────── */
        builder.Services.AddSingleton<IAuthenticationService,  AuthenticationService>();
        builder.Services.AddSingleton<INavigationService,      NavigationService>();
        builder.Services.AddScoped<IUserService,               UserService>();
        builder.Services.AddSingleton<IAppNotificationService, NotificationService>(); // ← helper service

        /* ───────── Shell & root ─────────── */
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        /* ───────── Pages & VMs ──────────── */
        builder.Services.AddTransient<MainViewModel>();        builder.Services.AddTransient<MainPage>();
        builder.Services.AddSingleton<LoginViewModel>();       builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<RegisterViewModel>();    builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<UserListViewModel>();    builder.Services.AddTransient<UserListPage>();
        builder.Services.AddTransient<UserDetailViewModel>();  builder.Services.AddTransient<UserDetailPage>();
        builder.Services.AddSingleton<TempViewModel>();        builder.Services.AddTransient<TempPage>();
        builder.Services.AddTransient<ProfileViewModel>();     builder.Services.AddTransient<Profile>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

#if ANDROID
        // fire‑and‑forget — do not block start‑up
        _ = RequestNotificationPermissionAsync();              // ← permission prompt (Tiramisu+)
#endif

#if DEBUG
        // Optional local‑notification diagnostics
        var notifCenter = LocalNotificationCenter.Current;
        notifCenter.NotificationReceived += args =>
            Debug.WriteLine($"[LocalNotif] received → {args?.Request?.Title}");
        notifCenter.NotificationActionTapped += args =>
            Debug.WriteLine($"[LocalNotif] tapped   → {args?.Request?.Title}");
#endif

        return app;
    }

    /// <summary>
    /// Requests POST_NOTIFICATIONS permission on Android 13 + (Tiramisu).
    /// On older versions this method is a no‑op.
    /// </summary>
   private static async Task RequestNotificationPermissionAsync()
{
#if ANDROID
    // Only Android 13 / API‑33 + need the runtime permission
    if (!OperatingSystem.IsAndroidVersionAtLeast(33))
        return;

    // MUST run on the main thread – the system dialog won’t open otherwise
    await MainThread.InvokeOnMainThreadAsync(async () =>
    {
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        if (status != PermissionStatus.Granted)
            await Permissions.RequestAsync<Permissions.PostNotifications>();
    });
#endif
}

}
