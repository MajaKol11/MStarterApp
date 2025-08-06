using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StarterApp.ViewModels
{
    //ViewModel for the main app shell, managing navigation and menus and reacting to login/logout events.
    public partial class AppShellViewModel : BaseViewModel
    {
        //Private field - meaning only this class can access it.
        //Readonly - meaning value can only be assigned once and cannot be changed later.
        //IAuthenticationService is an interface for managing user authentication, including login/logout and checking roles.
        private readonly IAuthenticationService _authService;

        //INavigationService is an interface for managing navigation within the application.
        private readonly INavigationService _navigationService;

        //A collection of menu items that can be changed at runtime.
        public ObservableCollection<MenuBarItem> DynamicMenuBarItems { get; } = new();

        //Constructor for AppShellViewModel.
        //Sets the app title.
        public AppShellViewModel()
        {
            Title = "StarterApp";
        }

        //Constructor for AppShellViewModel that takes dependencies.
        //Sets up event handlers for authentication state changes.
        //Initializes the title of the app shell.
        public AppShellViewModel(IAuthenticationService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            _authService.AuthenticationStateChanged += OnAuthenticationStateChanged;
            Title = "StarterApp";
        }

        //Returns true if the user has the "Guest" role, false otherwise.
        //This method is used to determine if guest actions can be executed.
        private bool CanExecuteGuestAction() => _authService.HasRole("Guest");

        //Returns true if the user has the "OrdinaryUser" role, false otherwise.
        //This method is used to determine if user actions can be executed.
        private bool CanExecuteUserAction() => _authService.HasRole("OrdinaryUser");
        
        //Returns true if the user has the "Admin" role, false otherwise.
        //This method is used to determine if admin actions can be executed.
        private bool CanExecuteAdminAction()
        {
            return _authService.HasRole("Admin");
        }
        
        //Returns true if the user is logged in, false otherwise.
        private bool CanExecuteAuthenticatedAction()
        {
            return _authService.IsAuthenticated;
        }
        
        //This method is called when the authentication state changes, meaning when a user logs in or out.
        private void OnAuthenticationStateChanged(object? sender, bool isAuthenticated)
        {
            LogoutCommand.NotifyCanExecuteChanged();
            NavigateToProfileCommand.NotifyCanExecuteChanged();
            NavigateToSettingsCommand.NotifyCanExecuteChanged();
            Debug.WriteLine($"Authentication state changed: {isAuthenticated}"); //For developers, not shown to the user.
            Debug.WriteLine($"Current user is admin: {_authService.HasRole("Admin")}");
        }

        //Opens the profile page 
        //Async method waits for task to complete before continuing.
        [RelayCommand]
        private async Task NavigateToProfileAsync()
        {
            await _navigationService.NavigateToAsync("TempPage");
        }

        //Opens the current user's settings page.
        [RelayCommand]
        private async Task NavigateToSettingsAsync()
        {
            await _navigationService.NavigateToAsync("TempPage");
        }

        //Logs user out and navigates to the login page.
        //Only works if user is logged in.
        [RelayCommand(CanExecute = nameof(CanExecuteAuthenticatedAction))]
        private async Task LogoutAsync()
        {
            await _authService.LogoutAsync(); //'Await' makes the program wait for the logout to complete before continuing.
            await _navigationService.NavigateToAsync("LoginPage"); //If 'await' was removed, both tasks would start and navigation could happen before logout completes.

            LogoutCommand.NotifyCanExecuteChanged();
            NavigateToProfileCommand.NotifyCanExecuteChanged();
            NavigateToSettingsCommand.NotifyCanExecuteChanged();
        }
    }
}