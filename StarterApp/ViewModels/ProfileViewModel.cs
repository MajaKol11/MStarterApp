/// @file ProfileViewModel.cs
/// @brief User‑profile management view‑model
/// @author StarterApp Development Team
/// @date   2025

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

/// <summary>
/// View‑model for the Profile page: bio editing + password change.
/// </summary>
public partial class ProfileViewModel : BaseViewModel
{
    /* ───────────────────────── dependencies ───────────────────────────── */

    private readonly IAuthenticationService   _authService;
    private readonly INavigationService       _navigationService;
    private readonly IUserService             _userService;
    private readonly IAppNotificationService  _notificationService;

    /* ───────────────────────── bindable state ─────────────────────────── */

    [ObservableProperty] private User?   currentUser;

    /* password */
    [ObservableProperty] private string  currentPassword   = string.Empty;
    [ObservableProperty] private string  newPassword       = string.Empty;
    [ObservableProperty] private string  confirmNewPassword = string.Empty;
    [ObservableProperty] private bool    isChangingPassword;

    /* bio */
    [ObservableProperty] private string? bio;
    [ObservableProperty] private string  editableBio = string.Empty;
    [ObservableProperty] private bool    editMode;

    /* ───────────────────────── constructor ────────────────────────────── */

    public ProfileViewModel(
        IAuthenticationService  authService,
        INavigationService      navigationService,
        IUserService            userService,
        IAppNotificationService notificationService)
    {
        _authService         = authService;
        _navigationService   = navigationService;
        _userService         = userService;
        _notificationService = notificationService;

        Title = "Profile";

        LoadUserData();
        _ = LoadBioAsync();
    }

    private void LoadUserData() => CurrentUser = _authService.CurrentUser;

    /* ───────────────────────── password section ───────────────────────── */

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (IsBusy || !ValidatePasswordChange()) return;

        try
        {
            IsBusy = true;
            ClearError();

            bool ok = await _authService.ChangePasswordAsync(CurrentPassword, NewPassword);

            if (ok)
            {
                await Application.Current.MainPage!.DisplayAlert(
                    "Success", "Password changed successfully!", "OK");

                ClearPasswordFields();
                IsChangingPassword = false;
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
        CurrentPassword   = string.Empty;
        NewPassword       = string.Empty;
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
            Bio             = EditableBio;
            EditMode        = false;

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
