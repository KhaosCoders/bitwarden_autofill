using Microsoft.UI;
using Microsoft.UI.Xaml;
using static Bitwarden_Autofill.Hotkey.Win32Api;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.System;
using Microsoft.Extensions.Options;
using System;
using Bitwarden_Autofill.Options;

namespace Bitwarden_Autofill.Hotkey;

/// <summary>
/// Represents a global hotkey manager that allows registering and handling global hotkeys.
/// </summary>
internal class GlobalHotkey
{
    private readonly IOptionsSnapshot<AppSettings> _options;
    private readonly Window _window;
    private readonly nint _hwnd;
    private int registeredHotkeyId;
    private string? registeredHotkey;

    public event EventHandler? HotkeyPressed;

    public GlobalHotkey(IOptionsSnapshot<AppSettings> options)
    {
        _options = options;

        _window = new();
        _hwnd = Win32Interop.GetWindowFromWindowId(_window.AppWindow.Id);

        // hook this window's message
        var hook = new WindowMessageHook(_window);
        _window.Closed += (s, e) => hook.Dispose(); // unhook on close
        hook.Message += (s, e) =>
        {
            if (e.Message == WM_HOTKEY)
            {
                OnHotkey();
            }
        };
    }

    public void RegisterHotkey()
    {
        var settings = _options.Value;
        var hotkey = settings.GlobalHotkey;

        if (string.Equals(registeredHotkey, hotkey, StringComparison.Ordinal)) return;

        (Modifiers modifiers, VirtualKey key) = ParseHotkey(hotkey);

        UnregisterHotkey();
        registeredHotkeyId = 1;
        if (!RegisterHotKey(_hwnd, registeredHotkeyId, modifiers, key))
            throw new Win32Exception(Marshal.GetLastWin32Error());
        registeredHotkey = hotkey;
    }

    public void UnregisterHotkey()
    {
        if (registeredHotkeyId == 0) return;
        if (!UnregisterHotKey(_hwnd, registeredHotkeyId))
            throw new Win32Exception(Marshal.GetLastWin32Error());
        registeredHotkeyId = 0;
    }

    private void OnHotkey()
    {
        Log.Information("Global Hotkey pressed");
        HotkeyPressed?.Invoke(this, EventArgs.Empty);
    }

    private static (Modifiers Modifiers, VirtualKey Key) ParseHotkey(ReadOnlySpan<char> hotkey)
    {
        Modifiers modifiers = 0;
        int index = hotkey.IndexOf('+');
        while (index != -1)
        {
            var modifier = hotkey[..index].Trim();
            if (Enum.TryParse<Modifiers>(modifier, true, out var mod))
            {
                modifiers |= mod;
            }
            else
            {
                Log.Warning("Unknown modifier: {Modifier}", modifier.ToString());
            }
            hotkey = hotkey[(index + 1)..];
            index = hotkey.IndexOf('+');
        }

        var key = Enum.Parse<VirtualKey>(hotkey.Trim().ToString(), true);
        return (modifiers, key);
    }
}
