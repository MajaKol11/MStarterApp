using StarterApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

//ViewModel for the main dashboard page (main screen after login).
//Extends BaseViewModel and inherits common properties and methods.
public partial class MainViewModel : BaseViewModel
{
    //Manages user authentication and session.
    private readonly IAuthenticationService _authService;

    //Navigation service for managing page navigation
    private readonly INavigationService _navigationService;

    //The user who is currently logged in.
    [ObservableProperty]
    private User? currentUser;

    // Welcome message to greet the user on the dashboard.
    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    //True if the user has the Admin role.
    //Used to show or hide admin features in the UI.
    private bool isAdmin;

    //Empty constructor for design time support.
    public MainViewModel()
    {
        //'Title' inherited from BaseViewModel and set to "Dashboard".'
        Title = "Dashboard";
    }

    //Main constructor that runs when the app is started.
    //It stores the services and loads user data.
    public MainViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Dashboard";

        LoadUserData();
    }

    //Loads the current user data from the authentication service.
    //Sets the welcome message based on the user's full name.
    private void LoadUserData()
    {
        CurrentUser = _authService.CurrentUser;
        IsAdmin = _authService.HasRole("Admin");

        if (CurrentUser != null) //If the current user is not null, set the welcome message.
        {
            WelcomeMessage = $"Welcome, {CurrentUser.FullName}!";
        }
    }

    //Navigates to the Profile page.
    [RelayCommand]
    private async Task NavigateToProfileAsync() =>
        await Shell.Current.GoToAsync(nameof(Profile));


    //Logs the user out after showing a confirmation dialog.
    //If the user confirms, it calls the logout method and navigates to the login page
    [RelayCommand]
    private async Task LogoutAsync()
    {
        var result = await Application.Current.MainPage.DisplayAlert( //This is the confirmation dialog. 'Await' makes the program wait for the user's response.
            "Logout",                                                 //This is the title of the dialog.
            "Are you sure you want to logout?",                       //This is the question displayed in the dialog.
            "Yes",                                                    //This is the text for the confirmation button.
            "No");                                                    //This is the text for the cancellation button.

        if (result) //If yes, proceed with logout. If no, do nothing.
        {
            await _authService.LogoutAsync();
            await _navigationService.NavigateToAsync("LoginPage");
        }
    }

    //Navigates to the settings page.
    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        await _navigationService.NavigateToAsync("TempPage");
    }

    //Navigates to the user list page (Admin-only feature).
    //If the user is not an admin, it shows an access denied message.
    [RelayCommand]
    private async Task NavigateToUserListAsync()
    {
        if (!IsAdmin)
        {
            await Application.Current.MainPage.DisplayAlert("Access Denied", "You don't have permission to access admin features.", "OK");
            return;
        }

        await _navigationService.NavigateToAsync("UserListPage");
    }

    //Refresh the dashboard data.
    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        try
        {
            IsBusy = true; //Set IsBusy to true to show loading state.
            LoadUserData(); //Reload user info.

            await Task.Delay(1000); //Simulate a short delay (1 second).
        }
        catch (Exception ex)
        {
            SetError($"Failed to refresh data: {ex.Message}");
        }
        finally
        {
            IsBusy = false; //Stop loading.
        }
    }
}