using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class Profile : ContentPage
{
    
    public Profile(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
