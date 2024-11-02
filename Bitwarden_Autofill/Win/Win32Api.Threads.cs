using System;
using System.Runtime.InteropServices;

namespace Bitwarden_Autofill.Win;

internal partial class Win32Api
{
    /// <summary>
    /// Retrieves the thread identifier of the calling thread.
    /// </summary>
    /// <returns>The thread identifier of the calling thread.</returns>
    [LibraryImport("kernel32.dll")]
    public static partial uint GetCurrentThreadId();

    /// <summary>
    /// Attaches or detaches the input processing mechanism of one thread to that of another thread.
    /// </summary>
    /// <param name="idAttach">The identifier of the thread to be attached or detached.</param>
    /// <param name="idAttachTo">The identifier of the thread to which idAttach will be attached or detached.</param>
    /// <param name="fAttach">If true, attaches the input processing mechanism; if false, detaches it.</param>
    /// <returns>True if the function succeeds; otherwise, false.</returns>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AttachThreadInput(uint idAttach, uint idAttachTo, [MarshalAs(UnmanagedType.Bool)] bool fAttach);

    /// <summary>
    /// Retrieves the thread identifier of the specified window's owner thread.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="lpdwProcessId">A pointer to a variable that receives the process identifier.</param>
    /// <returns>The thread identifier of the specified window's owner thread.</returns>
    [LibraryImport("user32.dll")]
    public static partial uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
}
