using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.System;

namespace Bitwarden_Autofill.Controls;

/// <summary>
/// A textbox that allows the user to select a hotkey.
/// </summary>
internal sealed partial class HotkeySelector : TextBox
{
    public HotkeySelector()
    {
        IsReadOnly = true;
    }

    public string Warning
    {
        get { return (string)GetValue(WarningProperty); }
        set { SetValue(WarningProperty, value); }
    }
    public static readonly DependencyProperty WarningProperty =
        DependencyProperty.Register("Warning", typeof(string), typeof(HotkeySelector), new PropertyMetadata(""));

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);
        e.Handled = true;
        var key = e.Key;
        switch (key)
        {
            case VirtualKey.Escape: return;
            case VirtualKey.Control: return;
            case VirtualKey.Shift: return;
            case VirtualKey.Menu: return;
            case VirtualKey.Back: return;
            case VirtualKey.Cancel: return;
            case VirtualKey.Clear: return;
            case VirtualKey.Delete: return;
            case VirtualKey.End: return;
            case VirtualKey.Home: return;
            case VirtualKey.Tab: return;
            case VirtualKey.LeftWindows: return;
            case VirtualKey.RightWindows: return;
        }

        var keys = new List<string>();
        if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
        {
            keys.Add("Ctrl");
        }
        if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down))
        {
            keys.Add("Alt");
        }
        if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
        {
            keys.Add("Shift");
        }
        keys.Add(key.ToString());

        Text = string.Join(" + ", keys);

        if (systemHotkeys.Contains(Text))
        {
            Warning = "This hotkey is reserved by the system.";
        }
        else if (keys.Count == 1)
        {
            Warning = "Single key hotkeys are dangerous.";
        }
        else
        {
            Warning = string.Empty;
        }
    }

    private HashSet<string> systemHotkeys =
        [
            "Ctrl + A",
            "Ctrl + B",
            "Ctrl + C",
            "Ctrl + D",
            "Ctrl + E",
            "Ctrl + F",
            "Ctrl + F3",
            "Ctrl + F4",
            "Ctrl + H",
            "Ctrl + I",
            "Ctrl + L",
            "Ctrl + N",
            "Ctrl + M",
            "Ctrl + U",
            "Ctrl + V",
            "Ctrl + X",
            "Ctrl + Y",
            "Ctrl + Z",
            "Ctrl + Left",
            "Ctrl + Right",
            "Ctrl + Up",
            "Ctrl + Down",
            "Ctrl + PageUp",
            "Ctrl + PageDown",
            "Ctrl + F5",
            "Ctrl + R",
            "Ctrl + W",
            "Ctrl + Space",
            "Ctrl + Shift + E",
            "Ctrl + Shift + N",
            "Ctrl + Shift + V",
            "Ctrl + Shift + Number1",
            "Ctrl + Shift + Number2",
            "Ctrl + Shift + Number3",
            "Ctrl + Shift + Number4",
            "Ctrl + Shift + Number5",
            "Ctrl + Shift + Number6",
            "Ctrl + Shift + Number7",
            "Ctrl + Shift + Number8",
            "Ctrl + Shift + Left",
            "Ctrl + Shift + Right",
            "Ctrl + Shift + Up",
            "Ctrl + Shift + Down",
            "Shift + Left",
            "Shift + Right",
            "Shift + Up",
            "Shift + Down",
            "Shift + F10",
            "Shift + Number1",
            "Shift + Number2",
            "Shift + Number3",
            "Shift + Number4",
            "Shift + Number5",
            "Shift + Number6",
            "Shift + Number7",
            "Shift + Number8",
            "Shift + Number9",
            "Alt + A",
            "Alt + D",
            "Alt + P",
            "Alt + Enter",
            "Alt + F4",
            "Alt + F8",
            "Alt + Left",
            "Alt + Right",
            "Alt + Up",
            "Alt + Down",
            "Alt + PageUp",
            "Alt + PageDown",
            "Alt + Space",
            "Alt + Shift + Left",
            "Alt + Shift + Right",
            "Alt + Shift + Up",
            "Alt + Shift + Down",
            "F11",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F10",
            "PageUp",
            "PageDown",
            "Up",
            "Down",
            "Left",
            "Right",
        ];
}
