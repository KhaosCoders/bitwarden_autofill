using System;
using System.IO;

namespace Bitwarden_Autofill.Bitwarden;

/// <summary>
/// Helps with finding bw.exe and adding it to the PATH.
/// </summary>
internal static class CliLocation
{
    public static string CliDirectory { get; } =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Bitwarden CLI");

    public static void EnsurePath()
    {
        if (!Directory.Exists(CliDirectory))
        {
            Directory.CreateDirectory(CliDirectory);
        }

        string PATH = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        if (!PATH.Contains(CliDirectory))
        {
            Environment.SetEnvironmentVariable("PATH", $"{PATH};{CliDirectory}");
        }
    }
}
