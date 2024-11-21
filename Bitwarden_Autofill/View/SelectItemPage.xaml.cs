using Bitwarden_Autofill.API.Models;
using Bitwarden_Autofill.Flow;
using Bitwarden_Autofill.Totp;
using Bitwarden_Autofill.ViewModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using System.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.System;

namespace Bitwarden_Autofill.View;

/// <summary>
/// A page that allows the user to select an item from a list of search results.
/// </summary>
internal sealed partial class SelectItemPage : Page, IPageWithSearch
{
    public SelectItemPage(SearchFlow searchFlow, PasswordFlow passwordFlow)
    {
        SearchFlow = searchFlow;
        PasswordFlow = passwordFlow;

        ViewModel = new()
        {
            UsernameCommand = UsernameClickedCommand,
            PasswordCommand = PasswordClickedCommand,
            TotpCommand = TotpClickedCommand,
            ToggleLinkCommand = ToggleLinkCommand,
        };
        ViewModel.ItemSelected += ViewModel_ItemSelected;
        InitializeComponent();
    }

    public SelectItemViewModel ViewModel { get; }

    private SearchFlow SearchFlow { get; set; }
    private PasswordFlow PasswordFlow { get; set; }

    private CancellationTokenSource? cancellationTokenSource;

    public void SearchTextChanged(string text)
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new();
        var token = cancellationTokenSource.Token;

        Task.Run(() => SearchFlow.SearchAsync(text, token)).LogErrors();
    }

    private void ViewModel_ItemSelected(BitwardenItem? item)
    {
        Log.Information("Item selected: {Item}", item?.Name);
        if (item?.Login == null) return;

        PasswordFlow.EnterCredentials(item);
    }

    [RelayCommand]
    private void OnUsernameClicked(BitwardenLogin? login)
    {
        if (login?.Username != null)
        {
            InputText(login.Username);
        }
    }


    [RelayCommand]
    private void OnPasswordClicked(BitwardenLogin? login)
    {
        if (login?.Password != null)
        {
            InputText(login.Password);
        }
    }


    [RelayCommand]
    private void OnTotpClicked(BitwardenLogin? login)
    {
        if (login?.Totp != null && TotpGenerator.FromUri(login.Totp) is string totp)
        {
            InputText(totp);
        }
    }

    [RelayCommand]
    private void OnToggleLink(BitwardenItem? item)
    {
        // TODO: Toggle Link
    }

    private void InputText(string text)
    {
        // Check if CTRL Key is pressed right now
        if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
        {
            // Copy text to clipboard
            DataPackage dataPackage = new();
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }
        else
        {
            // Type text
            // TODO: Type text
        }
    }
}
