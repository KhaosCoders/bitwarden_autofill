using Bitwarden_Autofill.Bitwarden.Model;
using System;
using System.Collections.Generic;

namespace Bitwarden_Autofill.Bitwarden;

internal class VaultSearch(BitwardenCli cli)
{
    public Task<IReadOnlyList<BitwardenItem>> FindItemsForUri(string uri) =>
        ListItems($"--url {uri}");

    public Task<IReadOnlyList<BitwardenItem>> FindItems(string searchText) =>
        ListItems($"--search {searchText}");

    private async Task<IReadOnlyList<BitwardenItem>> ListItems(string args)
    {
        var result = await cli.RunCommandAsync($"list items {args}", timeout: TimeSpan.FromSeconds(20));
        if (!result.Success || string.IsNullOrEmpty(result.Output))
        {
            return [];
        }

        return CliSerializer.Deserialize<ListResult<BitwardenItem>>(result.Output) ?? [];
    }


}
