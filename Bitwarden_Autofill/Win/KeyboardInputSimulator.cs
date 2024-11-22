using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Bitwarden_Autofill.Win.Win32Api;

namespace Bitwarden_Autofill.Win;

/// <summary>
/// Simulates keyboard input events.
/// </summary>
internal static class KeyboardInputSimulator
{
    /// <summary>
    /// Simulates all keyboard input events for the given text.
    /// The foreground process will receive the input.
    /// </summary>
    /// <param name="text">Text to send</param>
    public static void SendText(string text)
    {
        foreach (char c in text)
        {
            ushort keyCode = (ushort)VkKeyScan(c);
            SendKeyPress(keyCode);
        }
    }

    /// <summary>
    /// Sends a tab key press event.
    /// </summary>
    public static void SendTabKey() => SendKeyPress(0x09);

    /// <summary>
    /// Sends an enter key press event.
    /// </summary>
    public static void SendEnterKey() => SendKeyPress(0x0D);

    private static void SendKeyPress(ushort keyCode)
    {
        ushort vkCode = (ushort)(keyCode & 0xff);
        bool shift = (keyCode & 0x100) > 0;
        bool ctrl = (keyCode & 0x200) > 0;
        bool alt = (keyCode & 0x400) > 0;

        List<INPUT> inputs = [];

        void keydown(ushort keycode) =>
            inputs.Add(new INPUT
            {
                type = INPUT_KEYBOARD,
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = keycode,
                        dwFlags = 0
                    }
                }
            });

        void keyup(ushort keycode) =>
            inputs.Add(new INPUT
            {
                type = INPUT_KEYBOARD,
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = keycode,
                        dwFlags = KEYEVENTF_KEYUP
                    }
                }
            });

        if (shift) keydown(VK_SHIFT);
        if (ctrl) keydown(VK_CONTROL);
        if (alt) keydown(VK_MENU);

        keydown(vkCode);
        keyup(vkCode);

        if (shift) keyup(VK_SHIFT);
        if (ctrl) keyup(VK_CONTROL);
        if (alt) keyup(VK_MENU);

        SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
    }

}
