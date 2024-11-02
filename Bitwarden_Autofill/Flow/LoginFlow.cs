using Bitwarden_Autofill.API.Models;
using Bitwarden_Autofill.CLI;
using Bitwarden_Autofill.View;
using System;

namespace Bitwarden_Autofill.Flow;

internal class LoginFlow(BitwardenCli cli, UIDispatcher dispatcher)
{
    public async Task<bool> NeedsLogin()
    {
        var status = await cli.GetCliStatusAsync();
        return status.Status == EBitwardenStatus.Unauthenticated;
    }

    public async Task ShowStartPageAfterLogin(AppFlow flow)
    {
        if (await NeedsLogin())
        {
            Log.Information("Unauthenticated cli => login needed");
            dispatcher.ShowPage<LoginPage>();
        }
        else
        {
            Log.Debug("Already authenticated => no login needed");
            await flow.ShowStartPageAsync();
        }
    }
}
