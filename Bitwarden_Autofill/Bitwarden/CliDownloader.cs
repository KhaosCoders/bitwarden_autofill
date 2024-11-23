using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace Bitwarden_Autofill.Bitwarden;

/// <summary>
/// Dowloads the bitwarden CLI and extracts it to the CliDirectory.
/// </summary>
internal static class CliDownloader
{
    private const string _cliDownloadUrl = "https://vault.bitwarden.com/download/?app=cli&platform=windows";

    public delegate void DownloadProgressHandler(long totalBytes, long bytesDownloaded, bool indetermined);

    public static async Task DownloadCliAsync(DownloadProgressHandler? progressHandler = default)
    {
        Log.Information("Downloading bitwarden cli...");

        // Download the CLI
        using var client = new HttpClient();
        var response = await client.GetAsync(_cliDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        long totalBytes = response.Content.Headers.ContentLength ?? -1L;
        bool canReportProgress = totalBytes != -1 && progressHandler != default;

        await using var stream = await response.Content.ReadAsStreamAsync();
        await using var memoryStream = new MemoryStream();
        var buffer = new byte[8192];
        long totalRead = 0;
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
        {
            await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalRead += bytesRead;

            if (canReportProgress)
            {
                progressHandler?.Invoke(totalBytes, totalRead, false);
            }
        }

        Log.Debug("bitwarden cli downloaded");

        if (canReportProgress)
        {
            progressHandler?.Invoke(totalBytes, totalRead, true);
        }

        // Extract the zip file to the CliDirectory
        Log.Information("Extract bitwarden cli...");
        memoryStream.Seek(0, SeekOrigin.Begin);
        using var archive = new ZipArchive(memoryStream);
        archive.ExtractToDirectory(CliLocation.CliDirectory, overwriteFiles: true);

        Log.Debug("bitwarden cli extracted");
    }
}
