using StarterApp.ViewModels;
using StarterApp.Views;

namespace StarterApp;

public partial class AppShell : Shell
{
	public AppShell(AppShellViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();



		Routing.RegisterRoute(nameof(RegisterPage),   typeof(RegisterPage));
		Routing.RegisterRoute(nameof(UserListPage),   typeof(UserListPage));
		Routing.RegisterRoute(nameof(UserDetailPage), typeof(UserDetailPage));
		Routing.RegisterRoute(nameof(AboutPage),      typeof(AboutPage));
		Routing.RegisterRoute(nameof(MyNewPage),      typeof(MyNewPage));
		Routing.RegisterRoute(nameof(LoginPage),      typeof(LoginPage));
		Routing.RegisterRoute(nameof(MainPage),       typeof(MainPage));
		Routing.RegisterRoute(nameof(Profile), typeof(Profile));
		Routing.RegisterRoute(nameof(TempPage),       typeof(TempPage));
		

	}
}
