using Bitwarden_Autofill.Flow;
using H.NotifyIcon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Input;
using System;

namespace Bitwarden_Autofill;

public partial class App
{
    public TaskbarIcon? TrayIcon { get; private set; }

    private void InitializeTrayIcon()
    {
        ((XamlUICommand)Resources["SettingsCommand"])
            .ExecuteRequested += SettingsCommand_ExecuteRequested;

        ((XamlUICommand)Resources["ExitApplicationCommand"])
            .ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

        TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
        TrayIcon.ForceCreate();
    }

    private void SettingsCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        services.GetRequiredService<AppFlow>()
            .ShowSettings();
    }

    private void ExitApplicationCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        Log.Information("Exit via TrayIcon requested");
        Environment.Exit(0);
    }
}
