using System;
using System.Runtime.InteropServices;

namespace Bitwarden_Autofill.Win;

internal partial class Win32Api
{
    /// <summary>
    /// Retrieves a handle to the foreground window (the window with which the user is currently working).
    /// </summary>
    [LibraryImport("user32.dll")]
    public static partial IntPtr GetForegroundWindow();

    /// <summary>
    /// Brings the thread that created the specified window into the foreground and activates the window.
    /// </summary>
    /// <param name="hWnd">A handle to the window that should be activated and brought to the foreground.</param>
    /// <returns>true if the window was brought to the foreground; otherwise, false.</returns>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetForegroundWindow(IntPtr hWnd);

    /// <summary>
    /// Retrieves the handle to the window that has the keyboard focus.
    /// </summary>
    [LibraryImport("user32.dll")]
    public static partial IntPtr GetFocus();

    /// <summary>
    /// Sets the keyboard focus to the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window that will receive the keyboard input.</param>
    /// <returns>A handle to the window that previously had the keyboard focus.</returns>
    [LibraryImport("user32.dll")]
    public static partial IntPtr SetFocus(IntPtr hWnd);

    /// <summary>
    /// Retrieves the dimensions of the bounding rectangle of the specified window.
    /// The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="lpRect">A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</param>
    /// <returns>true if the function succeeds; otherwise, false.</returns>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    /// <summary>
    /// Defines the coordinates of the upper-left and lower-right corners of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
