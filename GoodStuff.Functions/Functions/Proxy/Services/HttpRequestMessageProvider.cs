using System.Net.Http.Headers;
using System.Text;
using GoodStuff.Functions.Interfaces;
using GoodStuff.Functions.Models;
using Microsoft.Azure.Functions.Worker.Http;

namespace GoodStuff.Functions.Functions.Proxy.Services;

public class HttpRequestMessageProvider(
    ITokenProviderService tokenProvider) : IHttpRequestMessageProvider
{
    public async Task<HttpRequestMessage> GetHttpRequestMessage(HttpRequestData request, ApiRoute apiRoute,
        string endpoint)
    {
        var method = GetHttpMethod(request);
        var message = new HttpRequestMessage(method, $"{apiRoute.BaseUrl}{endpoint}");
        await TrySetBody(message, request);
        await SetAuthenticationHeader(message, apiRoute);

        return message;
    }

    private static async Task TrySetBody(HttpRequestMessage message, HttpRequestData request)
    {
       var content = await request.ReadAsStringAsync();
       if (!string.IsNullOrWhiteSpace(content))
        message.Content = new StringContent(content, Encoding.UTF8, "application/json");
    }

    private static HttpMethod GetHttpMethod(HttpRequestData request)
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