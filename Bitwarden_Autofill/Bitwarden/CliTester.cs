using System.ComponentModel;
using System.Diagnostics;

namespace Bitwarden_Autofill.Bitwarden;

internal static class CliTester
{

    /// <summary>
    /// Checks if the Bitwarden CLI is installed.
    /// </summary>
    public static bool CheckIfInstalled()
    {
        try
        {
            CliLocation.EnsurePath();
            Process.Start(new ProcessStartInfo("bw")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            });
            return true;
        }
        catch (Win32Exception)
        {
            return false;
        }
    }
}