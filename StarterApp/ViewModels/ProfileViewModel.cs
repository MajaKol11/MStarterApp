using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

//ViewModel for the profile page.
//It lets users manage their password and edit their bio.
//It extends BaseViewModel to inherit common properties and methods.
public partial class ProfileViewModel : BaseViewModel
{
    //Dependencies (Services) that this ViewModel uses.
    private readonly IAuthenticationService _authService; //For user info and password changes.
    private readonly INavigationService _navigationService; //For moving between pages.
    private readonly IUserService _userService; //For updating and fetching user bio.
    private readonly IAppNotificationService _notificationService; //For showing notifications.

   //Properties for the ViewModel.
   //Current logged-in user.
    [ObservableProperty] private User? currentUser; 

    //Password section.
    [ObservableProperty] private string currentPassword = string.Empty; //Current password entered by user.
    [ObservableProperty] private string newPassword = string.Empty; //New password entered by the user.
    [ObservableProperty] private string confirmNewPassword = string.Empty; //Confirm new password field.
    [ObservableProperty] private bool isChangingPassword; //True if the user is changing their password, false otherwise.

    //Bio section.
    [ObservableProperty] private string? bio; //Saved bio from the database.
    [ObservableProperty] private string editableBio = string.Empty; //Editable bio text while user is in edit mode.
    [ObservableProperty] private bool editMode; //True if the user is editing their bio, false otherwise.

    //Constructor.
    public ProfileViewModel(
        IAuthenticationService authService,
        INavigationService navigationService,
        IUserService userService,
        IAppNotificationService notificationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _userService = userService;
        _notificationService = notificationService;

        Title = "Profile";

        LoadUserData();      //Load basic user info.
        _ = LoadBioAsync();  //Load bio from database.
    }

    private void LoadUserData() => CurrentUser = _authService.CurrentUser;

    //Password section.
    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (IsBusy || !ValidatePasswordChange()) return; //If already busy or validation fails, do nothing.

        try
        {
            IsBusy = true;
            ClearError();

            bool ok = await _authService.ChangePasswordAsync(CurrentPassword, NewPassword); //Asks the auth service to change the password.

            if (ok)
            {
                await Application.Current.MainPage!.DisplayAlert(
                    "Success", "Password changed successfully!", "OK");

                ClearPasswordFields(); //Clear password fields after successful change.
                IsChangingPassword = false; //Exit password change mode.
            }
            else
            {
                SetError("Failed to change password. Please check your current password.");
            }
        }
        catch (Exception ex)
        {
            SetError($"Password change failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void TogglePasswordChangeMode()
    {
        IsChangingPassword = !IsChangingPassword;
        if (!IsChangingPassword)
        {
            ClearPasswordFields();
            ClearError();
        }
    }

    private bool ValidatePasswordChange()
    {
        if (string.IsNullOrWhiteSpace(CurrentPassword))
            return SetErrorAndFalse("Current password is required");

        if (string.IsNullOrWhiteSpace(NewPassword))
            return SetErrorAndFalse("New password is required");

        if (NewPassword.Length < 6)
            return SetErrorAndFalse("New password must be at least 6 characters long");

        if (NewPassword != ConfirmNewPassword)
            return SetErrorAndFalse("New passwords do not match");

        if (CurrentPassword == NewPassword)
            return SetErrorAndFalse("New password must be different from current password");

        return true;

        bool SetErrorAndFalse(string msg) { SetError(msg); return false; }
    }

    private void ClearPasswordFields()
    {
        CurrentPassword = string.Empty;
        NewPassword = string.Empty;
        ConfirmNewPassword = string.Empty;
    }

    /* ───────────────────────── bio section ────────────────────────────── */

    [RelayCommand]
    private async Task LoadBioAsync()
    {
        var user = _authService.CurrentUser;
        if (user is null) return;

        Bio = await _userService.GetBioAsync(user.Id);
    }

    [RelayCommand]
    private void ToggleEditMode()
    {
        EditMode = !EditMode;
        if (EditMode && CurrentUser is not null)
            EditableBio = CurrentUser.Bio ?? string.Empty;
    }

    [RelayCommand]
    private async Task SaveBioAsync()
    {
        if (IsBusy || CurrentUser is null) return;

        try
        {
            IsBusy = true;
            ClearError();

            if (await _userService.UpdateBioAsync(CurrentUser.Id, EditableBio))
            {
                CurrentUser.Bio = EditableBio;
                Bio = EditableBio;
                EditMode = false;

                await _notificationService.ShowAsync(
                    "Profile updated",
                    "Your biography was changed successfully.");
            }
            else
            {
                SetError("Unable to save bio.");
            }
        }
        catch (Exception ex)
        {
            SetError($"Unable to save bio: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }


    /* ───────────────────────── navigation ─────────────────────────────── */

    [RelayCommand]
    private Task NavigateBackAsync() => _navigationService.NavigateBackAsync();
}
