using Bitwarden_Autofill.Bitwarden;
using Bitwarden_Autofill.View;

namespace Bitwarden_Autofill.Flow;

internal class BitwardenCliDownloadFlow(UIDispatcher dispatcher)
{
    public async Task EnsureCliInstalledAsync()
    {
        if (CliTester.CheckIfInstalled())
        {
            Log.Debug("bitwarden cli is already installed");
            return;
        }

        var window = dispatcher.OpenWindow<DownloadCliWindow>();
        await CliDownloader.DownloadCliAsync(
            progressHandler: (long totalBytes, long bytesDownloaded, bool indetermined) =>
                dispatcher.Dispatch(() =>
                {
                    window.ViewModel.IsIndetermined = indetermined;
                    window.ViewModel.DownloadProgress = (int)(bytesDownloaded * 100 / totalBytes);
                }));

        dispatcher.CloseWindow(window);
    }
}
