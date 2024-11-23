using Bitwarden_Autofill.Bitwarden;
using Bitwarden_Autofill.View;
using System;

namespace Bitwarden_Autofill.Flow;

internal class LoginFlow(CliAuth auth, UIDispatcher dispatcher)
{
    public Task<bool> NeedsLogin() => auth.NeedsLogin();

    public async Task ShowStartPageAfterLogin(AppFlow flow)
    {
        if (await auth.NeedsLogin())
        {
            Log.Information("Unauthenticated cli => login needed");
            dispatcher.ShowPage<LoginPage>();
        }
        else
        {
            Log.Debug("Already authenticated => no login needed");
            flow.ShowStartPage();
        }
    }
}
