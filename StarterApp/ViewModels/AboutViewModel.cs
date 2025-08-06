using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace StarterApp.ViewModels;

//This is the ViewModel for the About page, providing information about the app.
public class AboutViewModel
{
    //The name of the app, taken from the app settings.
    public string Title => AppInfo.Name;

    //The version of the app, taken from the app settings.
    public string Version => AppInfo.VersionString;

    //The URL that points to .NET MAUI documentation.
    public string MoreInfoUrl => "https://aka.ms/maui";
    
    public string Message => "This app is written in XAML and C# with .NET MAUI.";
    
    //Command that opens the web link in a browser when clicked.
    public ICommand ShowMoreInfoCommand { get; }

    //Initializes the ShowMoreInfoCommand with an asynchronous method.
    //An asynchronous method runs without blocking the main thread, allowing the program to continue working on other tasks while waiting for it to finish.
    public AboutViewModel()
    {
        ShowMoreInfoCommand = new AsyncRelayCommand(ShowMoreInfo);
    }

    //Opens the MoreInfoUrl in the default web browser.
    async Task ShowMoreInfo() =>
        await Launcher.Default.OpenAsync(MoreInfoUrl);
}