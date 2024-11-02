using System.Runtime.InteropServices;
using System;

namespace Bitwarden_Autofill.Win;

internal partial class Win32Api
{
    /// <summary>
    /// Returns NULL if no monitor is found.
    /// </summary>
    public const int MONITOR_DEFAULTTONULL = 0;

    /// <summary>
    /// Retrieves a handle to the display monitor that contains a specified point.
    /// </summary>
    /// <param name="pt">A POINT structure that specifies the point of interest in virtual-screen coordinates.</param>
    /// <param name="dwFlags">Determines the function's return value if the point is not contained within any display monitor.</param>
    /// <returns>
    /// If the point is contained by a display monitor, the return value is an HMONITOR handle to that display monitor.
    /// If the point is not contained by any display monitor, the return value depends on the value of dwFlags.
    /// </returns>
    [LibraryImport("user32.dll")]
    public static partial IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

    /// <summary>
    /// Contains the coordinates of a point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// Retrieves information about a display monitor.
    /// </summary>
    /// <param name="hMonitor">A handle to the display monitor of interest.</param>
    /// <param name="lpmi">A reference to a MONITORINFO structure that receives information about the specified display monitor.</param>
    /// <returns>
    /// Returns true if the function succeeds, or false otherwise.
    /// </returns>
    [DllImport("user32.dll")]
    public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    /// <summary>
    /// Contains information about a display monitor.
    /// </summary>
    /// <remarks>
    /// The MONITORINFO structure contains information such as the size of the structure, the display monitor rectangle,
    /// the work area rectangle, and any flags that indicate attributes of the display monitor.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }
}
