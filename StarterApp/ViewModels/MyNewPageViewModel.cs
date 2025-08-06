using CommunityToolkit.Mvvm.ComponentModel;

namespace StarterApp.ViewModels;

public partial class MyNewPageViewModel : ObservableObject
{
    //Send a welcome message.
    [ObservableProperty] private string welcome = "Hello from MyNewPage!";
}
