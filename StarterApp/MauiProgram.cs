using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StarterApp.Database.Data;
using StarterApp.Services;
using StarterApp.ViewModels;
using StarterApp.Views;
using Microsoft.Extensions.Configuration;

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
            });

        // ────────── Infrastructure ──────────
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DevelopmentConnection")));

        // ────────── Core services ───────────
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<INavigationService,    NavigationService>();
        builder.Services.AddScoped<IUserService,             UserService>();

        // ────────── Shell & root  ───────────
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        // ────────── Pages & view‑models ─────
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddSingleton<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<UserListViewModel>();
        builder.Services.AddTransient<UserListPage>();

        builder.Services.AddTransient<UserDetailViewModel>();
        builder.Services.AddTransient<UserDetailPage>();

        builder.Services.AddSingleton<TempViewModel>();
        builder.Services.AddTransient<TempPage>();

        builder.Services.AddTransient<ProfileViewModel>();   
        builder.Services.AddTransient<Profile>();            

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
