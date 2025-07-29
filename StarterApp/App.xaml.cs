using StarterApp.ViewModels;

namespace StarterApp;

public partial class App : Application
{
	private readonly IServiceProvider _serviceProvider;
	public App(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		InitializeComponent();

		Routing.RegisterRoute(nameof(Views.MainPage), typeof(Views.MainPage));
		Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
		Routing.RegisterRoute(nameof(Views.RegisterPage), typeof(Views.RegisterPage));
		Routing.RegisterRoute(nameof(Views.UserListPage), typeof(Views.UserListPage));
		Routing.RegisterRoute(nameof(Views.UserDetailPage), typeof(Views.UserDetailPage));
		Routing.RegisterRoute(nameof(Views.TempPage), typeof(Views.TempPage));
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		// Let IoC give us the fully composed shell (with ViewModel injected)
		var shell = _serviceProvider.GetRequiredService<AppShell>();

		// Return a single Window that hosts that shell
		return new Window(shell);
	}
}
