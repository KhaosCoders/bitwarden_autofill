namespace Bitwarden_Autofill.Bitwarden;

internal class CliConfigurator(BitwardenCli cli)
{
    /// <summary>
    /// Gets the server configuration from the Bitwarden CLI.
    /// </summary>
    public async Task<string> GetConnectedServerAsync()
    {
        Log.Debug("Reading bitwarden server config...");
        var result = await cli.RunCommandAsync("config server");
        return result.Output;
    }

    /// <summary>
    /// Sets the server configuration in the Bitwarden CLI.
    /// </summary>
    /// <param name="url">Url of Bitwarden server to connect to</param>
    public Task SetConnectedServer(string url)
    {
        Log.Information("Setting server config");
        return cli.RunCommandAsync($"config server {url}");
    }
}
