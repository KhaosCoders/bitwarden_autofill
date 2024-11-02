using System;
using System.Drawing;
using System.Text;
using static Bitwarden_Autofill.Win.Win32Api;

namespace Bitwarden_Autofill.Win;

/// <summary>
/// Provides methods to interact with Windows processes
/// </summary>
internal class WinProcesses
{
    private nint hwnd;
    private uint threadId;
    private uint processId;

    /// <summary>
    /// Pins all other actions to the foreground process
    /// </summary>
    /// <returns>True if the process was successfully pinned; otherwise, false.</returns>
    public bool PinToForegroundProcess()
    {
        hwnd = 0;
        threadId = 0;
        processId = 0;

        // Get active window handle
        try
        {
            hwnd = GetForegroundWindow();
            Log.Debug("Active window handle: {Hwnd}", hwnd);
            if (hwnd == IntPtr.Zero)
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get active window handle");
            return false;
        }

        // Get process id by window handle
        try
        {
            threadId = GetWindowThreadProcessId(hwnd, out processId);
            Log.Debug("Process ID: {ProcessId}", processId);
            if (threadId == 0 || processId == 0)
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get process id by window handle");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Brings the pinned window to the front
    /// </summary>
    public void ReactivatePinnedWindow()
    {
        if (hwnd == IntPtr.Zero) return;
        SetForegroundWindow(hwnd);
    }

    /// <summary>
    /// Get the rectangle of the focused control
    /// </summary>
    /// <returns>A <see cref="Rectangle"/> representing the bounds of the focused control.</returns>
    public Rectangle GetFocusedControlRect()
    {
        var hwnd = RunWithAttachedInputThread(GetFocus);
        Log.Debug("Focused hWnd: {Hwnd}", hwnd);

        RECT rect = default;
        GetWindowRect(hwnd, ref rect);
        Log.Debug("Focused Rect: {@Rect}", rect);

        return new(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
    }

    /// <summary>
    /// Get executable path of process by process id
    /// </summary>
    /// <returns>The executable path of the pinned process, or null if the path could not be retrieved.</returns>
    public string? GetPinnedProcessPath()
    {
        nint hProcess = IntPtr.Zero;
        try
        {
            hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, processId);

            if (hProcess == IntPtr.Zero)
            {
                return default;
            }

            StringBuilder exeName = new(1024);
            var exeNameSize = exeName.Capacity;
            QueryFullProcessImageName(hProcess, 0, exeName, ref exeNameSize);

            Log.Debug("Process path: {ExeName}", exeName);

            return exeName.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get executable path of process by process id");
            return default;
        }
        finally
        {
            if (hProcess != IntPtr.Zero)
                CloseHandle(hProcess);
        }
    }

    /// <summary>
    /// Runs the specified function with the input thread attached.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="func">The function to run.</param>
    /// <returns>The result of the function.</returns>
    private T RunWithAttachedInputThread<T>(Func<T> func)
    {
        uint currentThreadId = GetCurrentThreadId();

        if (threadId == currentThreadId)
        {
            return func();
        }

        try
        {
            AttachThreadInput(currentThreadId, threadId, true);
            return func();
        }
        finally
        {
            AttachThreadInput(currentThreadId, threadId, false);
        }
    }
}
