using Bitwarden_Autofill.CLI;
using Bitwarden_Autofill.Flow;
using Bitwarden_Autofill.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;
using Windows.System;

namespace Bitwarden_Autofill.View;

/// <summary>
/// A page that allows the user to login to the Bitwarden CLI.
/// </summary>
internal sealed partial class LoginPage : Page
{
    public LoginPage(AppFlow flow, UIDispatcher dispatcher, BitwardenCli cli)
    {
        _flow = flow;
        _dispatcher = dispatcher;
        _cli = cli;

        InitializeComponent();
        ViewModel = new();
        LoadConnectedServerConfig();

        Loaded += OnLoaded;
    }

    private readonly AppFlow _flow;
    private readonly UIDispatcher _dispatcher;
    private readonly BitwardenCli _cli;

    public LoginViewModel ViewModel { get; set; }


    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ClientIdTextbox.Focus(FocusState.Programmatic);
    }

    private void LoadConnectedServerConfig()
    {
        Task.Run(async () =>
        {
            if (await _cli.GetServerConfigAsync() is not string server)
            {
                return;
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                // Remove protocol
                if (server.IndexOf("://") is int index && index != -1)
                {
                    server = server[(index + 3)..];
                }

                // Remove subdomain
                index = server.LastIndexOf('.');
                index = server.LastIndexOf('.', index - 1);
                string host = index != -1 ? server[(index + 1)..] : server;
                var selectedIndex = ViewModel.Servers.IndexOf(host);
                if (selectedIndex != -1)
                {
                    Log.Debug("Connected to server: {Server}", ViewModel.Servers[selectedIndex]);
                    ViewModel.SelectedServerIndex = selectedIndex;
                }
                else
                {
                    Log.Debug("Connected to custom server");
                    ViewModel.Servers.Insert(2, server);
                    ViewModel.SelectedServerIndex = 2;
                }
            });
        }).LogErrors();
    }

    private void SecurityKeysLink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
    {
        Task.Run(async () =>
        {
            var server = ViewModel.Servers[ViewModel.SelectedServerIndex];
            await Launcher.LaunchUriAsync(new Uri($"https://vault.{server}/#/settings/security/security-keys"));
        }).LogErrors();
    }

    private void Connect_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("Trying to login...");

        _dispatcher.IndicateLoading();
        ViewModel.ValidationError = string.Empty;

        Task.Run(async () =>
        {
            // configure server
            string server = ViewModel.Servers[ViewModel.SelectedServerIndex];
            if (ViewModel.SelectedServerIndex >= 2)
            {
                server = $"https://{server}";
                Log.Debug("Connecting to custom server");
            }
            else
            {
                Log.Debug("Connecting to server: {Server}", server);
            }
            await _cli.ConfigServer(server);

            // login to cli
            var (Success, Error) = await _cli.LoginApiKey(ViewModel.ClientId, ViewModel.ClientSecret);
            if (Success)
            {
                await _flow.ShowStartPageAsync();
            }
            else
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    ViewModel.ValidationError = Error;
                    _dispatcher.ShowPage(this);
                });
            }
        }).LogErrors();
    }
}
