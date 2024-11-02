using Bitwarden_Autofill.API;
using Bitwarden_Autofill.CLI;

namespace Bitwarden_Autofill.Flow;

internal class BitwardenCliServeFlow(BitwardenCli cli, BitwardenApi api, ClientFactory clientFactory)
{
    public async Task<bool> StartServe()
    {
        var result = await cli.StartBitwardenCliServerAsync();

        if (!result.Success)
        {
            return false;
        }

        Log.Information("Bitwarden CLI server started.");
        clientFactory.SetServiceUrl($"http://localhost:{result.Port}");

        return await api.TestApi();
    }
}
