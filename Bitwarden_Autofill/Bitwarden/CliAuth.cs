using Bitwarden_Autofill.Bitwarden.Model;

namespace Bitwarden_Autofill.Bitwarden;

internal class CliAuth(BitwardenCli cli)
{
    /// <summary>
    /// Checks if the user needs to log in.
    /// </summary>
    public async Task<bool> NeedsLogin()
    {
        var status = await GetCliStatusAsync();
        return status.Status == EBitwardenStatus.Unauthenticated;
    }

    /// <summary>
    /// Gets the status of the Bitwarden CLI.
    /// </summary>
    private async Task<CliStatus> GetCliStatusAsync()
    {
        Log.Debug("Reading bitwarden cli status...");
        var result = await cli.RunCommandAsync("status");
        if (string.IsNullOrWhiteSpace(result.Output))
        {
            Log.Error("Unable to read status! {Error}", result.Error);
            return new() { Status = EBitwardenStatus.Unknown };
        }

        Log.Debug("Status: {Status}", result.Output);
        return CliSerializer.Deserialize<CliStatus>(result.Output)
            ?? new() { Status = EBitwardenStatus.Unknown };
    }

    /// <summary>
    /// Logs in to the Bitwarden CLI using the given email and password.
    /// </summary>
    /// <param name="clientId">Client-ID</param>
    /// <param name="clientSecret">Client-Secret</param>
    public async Task<(bool Success, string Error)> LoginApiKey(string clientId, string clientSecret)
    {
        Log.Information("Logging in with api key");
        var result = await cli.RunCommandAsync("login --apikey", new()
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
        cli.SetAccessToken(result.Output);
        return (true, string.Empty);
    }
}
