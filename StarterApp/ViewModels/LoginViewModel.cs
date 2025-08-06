using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using StarterApp.Views;

namespace StarterApp.ViewModels;

//ViewModel for the login page.
//Extends BaseViewModel and inherits from it to get common properties and methods.
public partial class LoginViewModel : BaseViewModel
{
    //Used to log the user in and check credentials.
    private readonly IAuthenticationService _authService;

    //Navigation service to move between pages.
    private readonly INavigationService _navigationService;

    //Stores the user's email address.
    //Linked to the email input field in the UI.
    [ObservableProperty]
    private string email = string.Empty;

    //Stores the user's password.
    //Linked to the password input field in the UI.
    [ObservableProperty]
    private string password = string.Empty;

    //Stores whether the 'Remember Me' checkbox is checked.
    [ObservableProperty]
    private bool rememberMe;

    //True if a login operation is running, false otherwise.
    //Shows spinner and disables the login button while waiting.
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool _isBusy;

    //Default constructor for design time support.
    public LoginViewModel()
    {
        Title = "Login"; //'Title' inherited from BaseViewModel.
    }

    //Main constructor that runs when the app is started.
    public LoginViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Login";
    }

    //Method is called when the user clicks the login button.
    //It checks inputs, tries to log in, and shows success or error messages.
    //Uses async/await to run without blocking the UI.
    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) //If already busy, do nothing to avoid multiple clicks.
            return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) //Check if email or password is empty, if yes show error.
        {
            SetError("Please enter both email and password");
            return;
        }

        try
        {
            IsBusy = true; //Set IsBusy to true to show loading state.
            ClearError(); //Clear any previous error messages.

            var result = await _authService.LoginAsync(Email, Password); //Try to log in with the provided email and password.

            if (result.IsSuccess)
            {
                Shell.Current.GoToAsync(nameof(UserListPage)); //If credentials match, navigate to the user list page.
            }
            else
            {
                SetError(result.Message); //If credentials do not match, show error message.
            }
        }
        catch (Exception ex)
        {
            SetError($"Login failed: {ex.Message}"); //If an exception occurs, show the error message.
        }
        finally
        {
            IsBusy = false; //Set IsBusy to false to hide loading state.
        }
    }

    //Method runs when the user clicks the 'Register' button.
    //It navigates to the registration page.
    [RelayCommand]
    private async Task NavigateToRegisterAsync()
    {
        await _navigationService.NavigateToAsync("RegisterPage");
    }

    //Runs when the user clicks the 'Forgot Password' button.
    //Should navigate to the forgot password page.
    //Currently, it shows a message that this feature is not implemented.
    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        // TODO: Implement forgot password functionality
        await Application.Current.MainPage.DisplayAlert("Info", "Forgot password functionality not implemented yet", "OK");
    }
}