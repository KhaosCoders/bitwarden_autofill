using Bitwarden_Autofill.API.Models;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Bitwarden_Autofill.API;

public class BitwardenApi(ClientFactory clientFactory)
{
    private readonly ClientFactory _clientFactory = clientFactory;

    public async Task<IReadOnlyList<BitwardenItem>> FindItemsForUri(string uri)
    {
        try
        {
            RestRequest request = new("/list/object/items", Method.Get);
            request.AddParameter("url", uri);
            var response = await ExecuteAsync<ApiResult<ListResult<BitwardenItem>>>(request);

            if (response?.IsSuccessStatusCode == true && response?.Data?.Data?.Data != null)
            {
                return response.Data.Data.Data.AsReadOnly();
            }
            else
            {
                Log.Error("Request to {Endpoint} failed. {ErrorMessage}", request.Resource, response?.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while trying to find items for uri {Uri}", uri);
        }

        return [];
    }

    public async Task<IReadOnlyList<BitwardenItem>> FindItems(string searchText)
    {
        try
        {
            RestRequest request = new("/list/object/items", Method.Get);
            request.AddParameter("search", searchText);
            var response = await ExecuteAsync<ApiResult<ListResult<BitwardenItem>>>(request);

            if (response?.IsSuccessStatusCode == true && response?.Data?.Data?.Data != null)
            {
                return response.Data.Data.Data.AsReadOnly();
            }
            else
            {
                Log.Error("Request to {Endpoint} failed. {ErrorMessage}", request.Resource, response?.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while trying to find items for search text {SearchText}", searchText);
        }

        return [];
    }

    public async Task<bool> UnlockAsync(string password)
    {
        RestRequest request = new("/unlock", Method.Post);
        request.AddParameter("password", password);
        var response = await ExecuteAsync<ApiResult<object>>(request);
        return response?.IsSuccessStatusCode == true;
    }

    public async Task<bool> LockAsync()
    {
        RestRequest request = new("/lock", Method.Post);
        var response = await ExecuteAsync<ApiResult<object>>(request);
        return response?.IsSuccessStatusCode == true;
    }

    public async Task<BitwardenStatus?> GetStatusAsync()
    {
        RestRequest request = new("/status", Method.Get);
        var response = await ExecuteAsync<ApiResult<ObjectResult<BitwardenStatus>>>(request);

        if (response?.IsSuccessStatusCode == true && response?.Data?.Data != null)
        {
            return response.Data.Data.Template;
        }
        else
        {
            Log.Error("Request to {Endpoint} failed. {ErrorMessage}", "/status", response?.ErrorMessage);
        }

        return default;
    }


    private async Task<RestResponse<T>?> ExecuteAsync<T>(RestRequest request)
    {
        var client = _clientFactory.GetClient();
        var response = await client.ExecuteAsync<T>(request);
        _clientFactory.ReturnClient(client);
        return response;
    }
}
