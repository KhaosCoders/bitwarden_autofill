using Bitwarden_Autofill.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

namespace Bitwarden_Autofill.CLI;

/// <summary>
/// Calls the Bitwarden CLI to perform various operations.
/// </summary>
internal class BitwardenCli
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private string? _accessToken;

    /// <summary>
    /// Checks if the Bitwarden CLI is installed.
    /// </summary>
    public bool CheckIfInstalled()
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

    /// <summary>
    /// Gets the status of the Bitwarden CLI.
    /// </summary>
    public async Task<CliStatus> GetCliStatusAsync()
    {
        Log.Debug("Reading bitwarden cli status...");
        var result = await RunCliCommandAsync("status");
        if (string.IsNullOrWhiteSpace(result.Output))
        {
            Log.Error("Unable to read status! {Error}", result.Error);
            return new() { Status = EBitwardenStatus.Unknown };
        }

        Log.Debug("Status: {Status}", result.Output);
        return JsonSerializer.Deserialize<CliStatus>(result.Output, jsonSerializerOptions)
            ?? new() { Status = EBitwardenStatus.Unknown };
    }

    /// <summary>
    /// Gets the server configuration from the Bitwarden CLI.
    /// </summary>
    public async Task<string> GetServerConfigAsync()
    {
        Log.Debug("Reading bitwarden server config...");
        var result = await RunCliCommandAsync("config server");
        return result.Output;
    }

    /// <summary>
    /// Sets the server configuration in the Bitwarden CLI.
    /// </summary>
    /// <param name="url">Url of Bitwarden server to connect to</param>
    public Task ConfigServer(string url)
    {
        Log.Information("Setting server config");
        return RunCliCommandAsync($"config server {url}");
    }

    /// <summary>
    /// Logs in to the Bitwarden CLI using the given email and password.
    /// </summary>
    /// <param name="clientId">Client-ID</param>
    /// <param name="clientSecret">Client-Secret</param>
    public async Task<(bool Success, string Error)> LoginApiKey(string clientId, string clientSecret)
    {
        Log.Information("Logging in with api key");
        var result = await RunCliCommandAsync("login --apikey", new()
        {
            ["BW_CLIENTID"] = clientId,
            ["BW_CLIENTSECRET"] = clientSecret,
        });

        if (!result.Success || !string.IsNullOrWhiteSpace(result.Error))
        {
            Log.Error("Login failed! {Error}", result.Error);
            return (false, string.IsNullOrWhiteSpace(result.Error) ? "Login failed" : result.Error);
        }

        Log.Debug("Login successful");
        _accessToken = result.Output;
        return (true, string.Empty);
    }

    /// <summary>
    /// Start the Bitwarden CLI server.
    /// </summary>
    /// <returns>the server port</returns>
    public async Task<(bool Success, int Port)> StartBitwardenCliServerAsync()
    {
        int port = PortFinder.FindUnusedPort();

        Log.Debug("Starting Bitwarden CLI server on port: {Port}...", port);
        var result = await RunCliCommandAsync($"serve --port {port}", noExit: true, timeout: TimeSpan.FromMilliseconds(500));

        return (result.Success, port);
    }

    private async Task<CliResult> RunCliCommandAsync(string arguments, Dictionary<string, string?>? env = default, TimeSpan timeout = default, bool noExit = false)
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
