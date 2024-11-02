using Bitwarden_Autofill.CLI;
using Bitwarden_Autofill.View;

namespace Bitwarden_Autofill.Flow;

internal class BitwardenCliDownloadFlow(BitwardenCli cli, UIDispatcher dispatcher)
{
    public async Task EnsureCliInstalledAsync()
    {
        if (cli.CheckIfInstalled())
        {
            Log.Debug("bitwarden cli is already installed");
            return;
        }

        var window = dispatcher.OpenWindow<DownloadCliWindow>();
        await CliDownloader.DownloadCliAsync((long totalBytes, long bytesDownloaded, bool indetermined) =>
            dispatcher.Dispatch(() =>
            {
                window.ViewModel.IsIndetermined = indetermined;
                window.ViewModel.DownloadProgress = (int)(bytesDownloaded * 100 / totalBytes);
            }));

        dispatcher.CloseWindow(window);
    }
}
