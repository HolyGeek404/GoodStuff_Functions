using System.Net.Http.Headers;
using GoodStuff.Functions.Interfaces;
using GoodStuff.Functions.Models;
using Microsoft.Azure.Functions.Worker.Http;

namespace GoodStuff.Functions.Services;

public class HttpRequestMessageProvider(
    ITokenProviderService tokenProvider) : IHttpRequestMessageProvider
{
    public async Task<HttpRequestMessage> GetHttpRequestMessage(HttpRequestData request, ApiRoute apiRoute,
        string endpoint)
    {
        var method = GetHttpMethod(request);
        var message = new HttpRequestMessage(method, $"{apiRoute.BaseUrl}{endpoint}");
        await SetAuthenticationHeader(message, apiRoute);

        return message;
    }

    private HttpMethod GetHttpMethod(HttpRequestData request)
    {
        return request.Method switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "DELETE" => HttpMethod.Delete,
            "PUT" => HttpMethod.Put,
            _ => throw new ArgumentException($"Invalid HTTP method: '{request.Method}'.")
        };
    }

    private async Task SetAuthenticationHeader(HttpRequestMessage message, ApiRoute apiRoute)
    {
        var token = await tokenProvider.GetAccessToken(apiRoute.Scope);
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}