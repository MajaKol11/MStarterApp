using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StarterApp.ViewModels;

//A base ViewModel class that other ViewModels can inherit from.
//It provides common properties and methods for all ViewModels in the application.
//This helps to keep the code DRY (Don't Repeat Yourself) by avoiding duplication of common
//functionality across different ViewModels.
public partial class BaseViewModel : ObservableObject
{
    //True if the viewmodel is currently busy, like loading data or processing a task.
    //Used to show a loading indicator in the UI, such as a spinner or progress bar.
    [ObservableProperty]
    private bool isBusy;

    //The title of the current page or view.
    [ObservableProperty]
    private string title = string.Empty;

    // Error message to display when something goes wrong.
    [ObservableProperty]
    private string errorMessage = string.Empty;

    //True if there is an error message to display.
    [ObservableProperty]
    private bool hasError;

    //Sets the error message and updates the hasError property as true.
    protected void SetError(string message)
    {
        ErrorMessage = message;
        HasError = !string.IsNullOrEmpty(message);
    }

    //Clears the current error state.
    //Resets both ErrorMessage and HasError properties.
    protected void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    //Command that clears the error when called from the UI.
    //Command can be bound to a 'close' or 'dismiss' button in the UI.
    [RelayCommand]
    private void ClearErrorCommand()
    {
        ClearError();
    }
}