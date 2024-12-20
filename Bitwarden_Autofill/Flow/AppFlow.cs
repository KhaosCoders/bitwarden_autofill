﻿using Bitwarden_Autofill.Hotkey;
using Bitwarden_Autofill.View;
using System;
using System.Linq;

namespace Bitwarden_Autofill.Flow;

internal class AppFlow(
    UIDispatcher dispatcher,
    BitwardenCliDownloadFlow cliFlow,
    LoginFlow loginFlow,
    PasswordFlow passwordFlow,
    GlobalHotkey hotkey)
{
    private static bool IsAutoRun =>
        Environment.GetCommandLineArgs().Contains("--autorun");

    public async Task AppLaunchAsync()
    {
        bool autoRun = IsAutoRun;
        Log.Debug("App launch. Autorun: {Autorun}", autoRun);

        // Ensure CLI is installed
        await cliFlow.EnsureCliInstalledAsync();

        // Just start background services in autorun mode
        if (autoRun && !await loginFlow.NeedsLogin())
        {
            HookUpGlobalHotkeyHandler();
            return;
        }

        // Open MainWindow
        dispatcher.OpenMainWindow();
        dispatcher.IndicateLoading();

        // Check Login status
        await loginFlow.ShowStartPageAfterLogin(this);
    }

    public void ShowStartPage()
    {
        Log.Information("Showing app start page");

        HookUpGlobalHotkeyHandler();

        // Show StartPage
        dispatcher.ShowPage<StartPage>();
    }

    public void HookUpGlobalHotkeyHandler()
    {
        Log.Information("Starting app services...");

        // Enable Global Hotkey
        hotkey.HotkeyPressed += OnGlobalHotkey;
        dispatcher.Dispatch(hotkey.RegisterHotkey);
    }

    private void OnGlobalHotkey(object? sender, EventArgs e)
    {
        passwordFlow.ShowPasswordSelectionAsync().LogErrors();
    }

    public void ShowSettings()
    {
        dispatcher.OpenMainWindow();
        dispatcher.ShowPage<StartPage>();
    }
}
