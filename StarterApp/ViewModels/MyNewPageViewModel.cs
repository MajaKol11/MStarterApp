using CommunityToolkit.Mvvm.ComponentModel;

namespace StarterApp.ViewModels;

public partial class MyNewPageViewModel : ObservableObject
{
    // Example bindable property
    [ObservableProperty] private string welcome = "Hello from MyNewPage!";
}
