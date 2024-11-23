using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bitwarden_Autofill.Bitwarden;

/// <summary>
/// Calls the Bitwarden CLI to perform various operations.
/// </summary>
internal class BitwardenCli
{
    private string? _accessToken;

    public void SetAccessToken(string accessToken) => _accessToken = accessToken;

    public async Task<CliResult> RunCommandAsync(string arguments, Dictionary<string, string?>? env = default, TimeSpan timeout = default, bool noExit = false)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = "bw",
            Arguments = $"{arguments} --raw --nointeraction --cleanexit",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        if (env != null)
        {
            foreach (var (key, value) in env)
            {
                startInfo.Environment[key] = value;
            }
        }

        if (!string.IsNullOrWhiteSpace(_accessToken))
        {
            startInfo.Environment["BW_SESSION"] = _accessToken;
        }

        var proc = Process.Start(startInfo);

        if (proc == null) return new(false, string.Empty, "Unable to start bw process", 1);

        if (timeout == default)
        {
            await proc.WaitForExitAsync();
        }
        else
        {
            if (!proc.WaitForExit(timeout))
            {
                if (noExit)
                {
                    return new(true, string.Empty, string.Empty, -1);
                }

                Log.Warning("Process timed out, killing...");

                proc.Kill();
                return new(false, string.Empty, "Process timed out", 1);
            }
        }

        string output = await proc.StandardOutput.ReadToEndAsync();
        string error = await proc.StandardError.ReadToEndAsync();

        return new(proc.ExitCode == 0, output, error, proc.ExitCode);
    }
}
