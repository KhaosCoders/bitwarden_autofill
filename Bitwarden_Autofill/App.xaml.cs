using Bitwarden_Autofill.Flow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace Bitwarden_Autofill;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private readonly ServiceProvider services;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        SetupLogging();
        Log.Information("Application v{Version} starting...",
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        services = ConfigureServices();
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        InitializeTrayIcon();

        // Start the main application flow
        Task.Run(
            services.GetRequiredService<AppFlow>().AppLaunchAsync
        ).LogErrors();
    }
}
