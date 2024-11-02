using System.Runtime.InteropServices;
using System;
using System.Text;

namespace Bitwarden_Autofill.Win;

internal partial class Win32Api
{
    /// <summary>
    /// Grants limited access rights to the process.
    /// </summary>
    public const int PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

    /// <summary>
    /// Opens an existing local process object.
    /// </summary>
    /// <param name="dwDesiredAccess">The access to the process object. This access right is checked against the security descriptor for the process.</param>
    /// <param name="bInheritHandle">If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
    /// <param name="dwProcessId">The identifier of the local process to be opened.</param>
    /// <returns>If the function succeeds, the return value is an open handle to the specified process. If the function fails, the return value is NULL.</returns>
    [LibraryImport("kernel32.dll")]
    public static partial IntPtr OpenProcess(uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

    /// <summary>
    /// Retrieves the full name of the executable image for the specified process.
    /// </summary>
    /// <param name="hProcess">A handle to the process. This handle must be created with the PROCESS_QUERY_LIMITED_INFORMATION access right.</param>
    /// <param name="dwFlags">This parameter can be zero or the following value: 0x00000000.</param>
    /// <param name="lpExeName">A pointer to a buffer that receives the full name of the executable image.</param>
    /// <param name="lpdwSize">On input, specifies the size of the lpExeName buffer, in characters. On output, receives the number of characters copied to the buffer, including the null-terminating character.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern int QueryFullProcessImageName(IntPtr hProcess, int dwFlags, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpExeName, ref int lpdwSize);
}
