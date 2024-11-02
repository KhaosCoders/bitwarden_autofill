using Bitwarden_Autofill.Flow;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static Bitwarden_Autofill.Win.Win32Api;

namespace Bitwarden_Autofill.Win;

/// <summary>
/// Moves the main window in proximity of the focused control.
/// </summary>
internal class WindowPlacement(UIDispatcher dispatcher, WinProcesses winProcesses)
{
    private const int rightPadding = 20;
    private const int bottomPadding = 60;

    private Rectangle focusedRect;

    /// <summary>
    /// Pins the move action to the currently focused control.
    /// </summary>
    public void PinToFocusedControl()
    {
        focusedRect = winProcesses.GetFocusedControlRect();
    }

    /// <summary>
    /// Moves the main window in proximity of the focused control.
    /// Ensures that the window is not off screen.
    /// </summary>
    public void MoveMainWindowInProximityOfFocusedControl()
    {
        // Position on the left side of the focused control
        var y = focusedRect.Y;
        var x = focusedRect.X - dispatcher.MainWindowWidth - rightPadding;

        // If the window is off screen, move it to the right side of the focused control
        if (IsOffScreen(new Point((int)x, y)))
        {
            x = focusedRect.Right;
        }

        // If the window is still off screen, move it to the right side of the focused control
        if (IsOffScreen(new Point((int)x, y)))
        {
            x = focusedRect.Right - dispatcher.MainWindowWidth - rightPadding;
        }

        // If the window is cut off at the bottom, move it up
        var windowRect = new Rectangle(new Point((int)x, y), new Size((int)dispatcher.MainWindowWidth, (int)dispatcher.MainWindowHeight));
        var monitorRect = GetMonitorRect(windowRect.Location);
        if (!monitorRect.IsEmpty && !monitorRect.Contains(windowRect))
        {
            y = monitorRect.Bottom - (int)dispatcher.MainWindowHeight - bottomPadding;
        }

        dispatcher.MoveMainWindow(new Point((int)x, y));
    }

    private bool IsOffScreen(Point point) => GetMonitor(point) == IntPtr.Zero;

    private IntPtr GetMonitor(Point point) => MonitorFromPoint(new() { X = point.X, Y = point.Y }, MONITOR_DEFAULTTONULL);

    private Rectangle GetMonitorRect(Point point) => GetMonitorRect(GetMonitor(point));

    private Rectangle GetMonitorRect(IntPtr monitor)
    {
        MONITORINFO monitorInfo = new() { cbSize = Marshal.SizeOf<MONITORINFO>() };
        if (GetMonitorInfo(monitor, ref monitorInfo))
        {
            return new(
                new Point(monitorInfo.rcMonitor.Left, monitorInfo.rcMonitor.Top),
                new Size(monitorInfo.rcMonitor.Right - monitorInfo.rcMonitor.Left, monitorInfo.rcMonitor.Bottom - monitorInfo.rcMonitor.Top));
        }
        return Rectangle.Empty;
    }
}
