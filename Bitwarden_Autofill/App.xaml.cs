using Bitwarden_Autofill.API;
using Bitwarden_Autofill.CLI;
using Bitwarden_Autofill.Flow;
using Bitwarden_Autofill.Hotkey;
using Bitwarden_Autofill.Win;
using Bitwarden_Autofill.View;
using H.NotifyIcon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using Bitwarden_Autofill.Options;

namespace Bitwarden_Autofill;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        SetupLogging();
        Log.Information("Application v{Version} starting...", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            Log.Fatal((Exception)args.ExceptionObject, "Unhandled exception");
        };

        UnhandledException += (sender, args) =>
        {
            Log.Fatal(args.Exception, "Unhandled exception");
        };

        Services = ConfigureServices();
        InitializeComponent();
    }

    private static void SetupLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .CreateLogger();
    }

    /// <summary>
    /// Gets the <see cref="ServiceProvider"/> instance to resolve application services.
    /// </summary>
    public ServiceProvider Services { get; }

    public TaskbarIcon? TrayIcon { get; private set; }

    private void InitializeTrayIcon()
    {
        var settingsCommand = (XamlUICommand)Resources["SettingsCommand"];
        settingsCommand.ExecuteRequested += SettingsCommand_ExecuteRequested;

        var exitApplicationCommand = (XamlUICommand)Resources["ExitApplicationCommand"];
        exitApplicationCommand.ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

        TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
        TrayIcon.ForceCreate();
    }

    private void SettingsCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        var flow = Services.GetRequiredService<AppFlow>();
        flow.ShowSettings();
    }

    private void ExitApplicationCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        Log.Information("Exit via TrayIcon requested");
        Environment.Exit(0);
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        InitializeTrayIcon();

        var flow = Services.GetRequiredService<AppFlow>();
        Task.Run(flow.AppLaunchAsync).LogErrors();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ClientFactory>();

        services.AddSingleton(DispatcherQueue.GetForCurrentThread());
        services.AddSingleton<UIDispatcher>();

        services.AddSingleton<GlobalHotkey>();
        services.AddSingleton<WinProcesses>();
        services.AddSingleton<WindowPlacement>();

        services.AddSingleton<IOptionsFactory<AppSettings>, SettingsStore>();
        services.AddSingleton<ISettingsStore, SettingsStore>();
        services.AddOptions<AppSettings>();

        services.AddSingleton<BitwardenCli>();
        services.AddSingleton<BitwardenApi>();
        services.AddTransient<DownloadCliWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<ErrorPage>();
        services.AddTransient<StartPage>();
        services.AddTransient<LoadingPage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<UnlockPage>();
        services.AddTransient<SelectItemPage>();

        services.AddSingleton<AppFlow>();
        services.AddSingleton<BitwardenCliDownloadFlow>();
        services.AddSingleton<BitwardenCliServeFlow>();
        services.AddSingleton<LoginFlow>();
        services.AddSingleton<PasswordFlow>();
        services.AddSingleton<SearchFlow>();

        return services.BuildServiceProvider();
    }
}
