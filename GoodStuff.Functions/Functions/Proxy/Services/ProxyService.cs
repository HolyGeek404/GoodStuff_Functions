using System.Text;
using GoodStuff.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GoodStuff.Functions.Functions.Proxy.Services;

public class ProxyService(
    IValidatorService validatorService,
    IHttpClientFactory httpClientFactory,
    ILogger<ProxyService> logger,
    IHttpRequestMessageProvider httpRequestMessageProvider) : IFunctionService
{
    public async Task<HttpResponseMessage> ProcessRequest(HttpRequestData request, string api, string endpoint)
    {
        logger.LogInformation("Started processing request. Api: {Api}, Endpoint: {Endpoint}", api, endpoint);

        try
        {
            // Validate API route
            var apiRoute = validatorService.ValidateApi(api);

            // Create HttpClient
            var httpClient = httpClientFactory.CreateClient(apiRoute.BaseUrl);

            // Build outgoing HttpRequestMessage
            var message = await httpRequestMessageProvider.GetHttpRequestMessage(request, apiRoute, endpoint);

            // Execute request
            var response = await httpClient.SendAsync(message);
            logger.LogInformation("Successfully processed request. Api: {Api}, Endpoint: {Endpoint}", api, endpoint);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in processing the request. Api: {Api}, Endpoint: {Endpoint}, ErrorMessage: {Message}", api, endpoint, ex.Message);
            throw; // preserve stack
        }
    }

    public async Task<HttpResponseData> HandleResponse(HttpResponseMessage response, HttpRequestData req)
    {
        logger.LogInformation("Handling response. StatusCode: {StatusCode}", response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var result = req.CreateResponse(response.StatusCode);
        result.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await result.WriteStringAsync(content, Encoding.UTF8);

        logger.LogInformation("Response handling completed.");
        return result;
    }
}