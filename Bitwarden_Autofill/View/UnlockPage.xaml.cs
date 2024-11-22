using Bitwarden_Autofill.API;
using Bitwarden_Autofill.Flow;
using Bitwarden_Autofill.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace Bitwarden_Autofill.View;

/// <summary>
/// A page that allows the user to unlock the Bitwarden vault.
/// </summary>
internal sealed partial class UnlockPage : Page
{
    public UnlockPage(BitwardenApi api, UIDispatcher dispatcher, PasswordFlow flow)
    {
        _api = api;
        _dispatcher = dispatcher;
        _flow = flow;

        ViewModel = new();
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private readonly BitwardenApi _api;
    private readonly UIDispatcher _dispatcher;
    private readonly PasswordFlow _flow;

    public UnlockViewModel ViewModel { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        PasswordTextbox.Focus(FocusState.Programmatic);
    }

    private void Unlock_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ErrorMessage = null;
        Task.Run(UnlockAsync).LogErrors();
    }

    private async Task UnlockAsync()
    {
        _dispatcher.IndicateLoading();

        if (!await _api.UnlockAsync(ViewModel.Password))
        {
            _dispatcher.Dispatch(() => ViewModel.ErrorMessage = "Invalid password");
            _dispatcher.ShowPage(this);
            return;
        }

        await _flow.ContinueAfterUnlockAsync();
    }

    private void PasswordKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            Unlock_Click(sender, e);
        }
    }
}
