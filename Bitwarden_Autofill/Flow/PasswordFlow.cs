using Bitwarden_Autofill.Bitwarden;
using Bitwarden_Autofill.Bitwarden.Model;
using Bitwarden_Autofill.View;
using Bitwarden_Autofill.Win;
using System;
using System.IO;

namespace Bitwarden_Autofill.Flow;

internal class PasswordFlow(
    UIDispatcher dispatcher,
    WinProcesses winProcesses,
    WindowPlacement desktopManager,
    VaultLock vaultLock,
    VaultSearch vaultSearch)
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
        if (await vaultLock.IsLockedAsync())
        {
            dispatcher.ShowPage<UnlockPage>();
            return;
        }

        await ContinueAfterUnlockAsync();
    }

    public Task ContinueAfterUnlockAsync()
    {
        dispatcher.IndicateLoading();
        return DisplayPasswordsForProcessAsync(targetProcessFileName);
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

    private async Task DisplayPasswordsForProcessAsync(string targetProcess = "")
    {
        var foundItems = await vaultSearch.FindItemsForUri(ProcessUri);
        dispatcher.ShowPage<SelectItemPage>(page =>
        {
            page.ViewModel.AttachedProcess = targetProcess;
            page.ViewModel.Items.Clear();
            foreach (var item in foundItems)
            {
                page.ViewModel.Items.Add(item);
            }
        });
    }
}
