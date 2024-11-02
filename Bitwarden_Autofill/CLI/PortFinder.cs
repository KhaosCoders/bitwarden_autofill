namespace Bitwarden_Autofill.CLI;

/// <summary>
/// Provides functionality to find an unused port on the local machine.
/// </summary>
internal static class PortFinder
{
    /// <summary>
    /// Finds an unused port on the local machine.
    /// </summary>
    /// <returns>An integer representing an unused port number.</returns>
    public static int FindUnusedPort()
    {
        Log.Debug("Looking for unused port...");
        var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        int port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        Log.Debug("Found unused port: {Port}", port);
        return port;
    }
}
