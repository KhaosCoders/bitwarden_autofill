using RestSharp;
using System.Collections.Concurrent;

namespace Bitwarden_Autofill.API;

public class ClientFactory
{
    private readonly ConcurrentStack<RestClient> _clients = new();
    private string? _serviceUrl;

    public RestClient GetClient()
    {
        if (_clients.TryPop(out var client))
        {
            return client;
        }

        if (!string.IsNullOrWhiteSpace(_serviceUrl))
        {
            return new RestClient(_serviceUrl);
        }
        return new RestClient();
    }

    public void ReturnClient(RestClient client)
    {
        _clients.Push(client);
    }

    public void SetServiceUrl(string url)
    {
        _serviceUrl = url;
        while (_clients.TryPop(out var client))
        {
            client.Dispose();
        }
    }
}
