using Bitwarden_Autofill.API;
using Bitwarden_Autofill.API.Models;
using Bitwarden_Autofill.Win;
using Bitwarden_Autofill.View;
using System;
using System.IO;

namespace Bitwarden_Autofill.Flow;

internal class PasswordFlow(
    UIDispatcher dispatcher,
    WinProcesses winProcesses,
    WindowPlacement desktopManager,
    BitwardenApi api)
{
    private string targetProcessFileName = string.Empty;
    private string ProcessUri => $"file://{targetProcessFileName}";

    public async Task ShowPasswordSelectionAsync()
    {
        winProcesses.PinToForegroundProcess();
        desktopManager.PinToFocusedControl();

        var targetProcessPath = winProcesses.GetPinnedProcessPath();
        targetProcessFileName = Path.GetFileName(targetProcessPath) ?? string.Empty;

        // Show loading page
        dispatcher.OpenMainWindow(targetProcessFileName);
        desktopManager.MoveMainWindowInProximityOfFocusedControl();
        dispatcher.IndicateLoading();

        // Unlock?
        if (await IsLockedAsync())
        {
            dispatcher.ShowPage<UnlockPage>();
            return;
        }

        await ContinueAfterUnlockAsync();
    }

    public Task ContinueAfterUnlockAsync()
    {
        dispatcher.IndicateLoading();
        return DisplayPasswordsForProcessAsync();
    }

    public void EnterCredentials(BitwardenItem item)
    {
        winProcesses.ReactivatePinnedWindow();
        dispatcher.HideMainWindow();

        bool hasSend = false;
        if (!string.IsNullOrEmpty(item.Login?.Username))
        {
            hasSend = true;
            KeyboardInputSimulator.SendText(item.Login.Username);
        }

        if (!string.IsNullOrEmpty(item.Login?.Password))
        {
            if (hasSend)
            {
                KeyboardInputSimulator.SendTabKey();
            }

            hasSend = true;
            KeyboardInputSimulator.SendText(item.Login.Password);
        }

        if (hasSend)
        {
            KeyboardInputSimulator.SendEnterKey();
        }
    }

    private async Task DisplayPasswordsForProcessAsync()
    {
        var foundItems = await api.FindItemsForUri(ProcessUri);
        dispatcher.ShowPage<SelectItemPage>(page =>
        {
            page.ViewModel.Items.Clear();
            foreach (var item in foundItems)
            {
                page.ViewModel.Items.Add(item);
            }
        });
    }

    private async Task<bool> IsLockedAsync()
    {
        var status = await api.GetStatusAsync();
        return status?.Status != EBitwardenStatus.Unlocked;
    }
}
