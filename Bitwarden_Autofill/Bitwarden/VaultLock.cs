namespace Bitwarden_Autofill.Bitwarden;

internal class VaultLock(BitwardenCli cli)
{
    public async Task<bool> IsLockedAsync()
    {
        var result = await cli.RunCommandAsync("unlock --check --raw");
        return !result.Success || !string.IsNullOrEmpty(result.Error);
    }

    public async Task<bool> UnlockAsync(string password)
    {
        var result = await cli.RunCommandAsync("unlock --passwordenv BW_PW --raw", new()
        {
            ["BW_PW"] = password,
        });

        if (!result.Success ||
            !string.IsNullOrWhiteSpace(result.Error) ||
            string.IsNullOrWhiteSpace(result.Output))
        {
            return false;
        }

        cli.SetAccessToken(result.Output);
        return true;
    }

    public Task LockAsync() =>
        cli.RunCommandAsync("lock");
}
