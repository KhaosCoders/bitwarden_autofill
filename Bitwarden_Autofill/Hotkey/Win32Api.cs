﻿using System;
using System.Runtime.InteropServices;
using Windows.System;

namespace Bitwarden_Autofill.Hotkey;

internal partial class Win32Api
{
    public const int WM_HOTKEY = 0x312;

    /// <summary>
    /// Registers a hot key with the system.
    /// </summary>
    /// <param name="hWnd">A handle to the window that will receive WM_HOTKEY messages generated by the hot key.</param>
    /// <param name="id">The identifier of the hot key.</param>
    /// <param name="fsModifiers">The keys that must be pressed in combination with the key specified by the <paramref name="vk"/> parameter in order to generate the WM_HOTKEY message.</param>
    /// <param name="vk">The virtual-key code of the hot key.</param>
    /// <returns>True if the function succeeds; otherwise, false.</returns>
    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool RegisterHotKey(nint hWnd, int id, Modifiers fsModifiers, VirtualKey vk);

    /// <summary>
    /// Unregisters a previously registered hot key.
    /// </summary>
    /// <param name="hWnd">A handle to the window associated with the hot key to be unregistered.</param>
    /// <param name="id">The identifier of the hot key to be unregistered.</param>
    /// <returns>True if the function succeeds; otherwise, false.</returns>
    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnregisterHotKey(nint hWnd, int id);

    [Flags]
    internal enum Modifiers
    {
        Alt = 0x1,
        Ctrl = 0x2,
        Shift = 0x4,
        Win = 0x8,
        NoRepeat = 0x4000,
    }
}
