using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GoodStuff.Functions;

public class ProductGatewayFunction(ILogger<ProductGatewayFunction> logger, IConfiguration config)
{
    private readonly ILogger _logger = logger;
    private readonly string _apiBaseUrl = config["ProductApi:BaseUrl"] ?? "https://localhost:7003/Product";
    private readonly string _entraResource = config["ProductApi:EntraResource"] ?? "api://56b1c593-a584-4622-b223-bcf0fb117cb1/.default";
    private readonly string[] _allowedCategories = (config["ProductApi:AllowedCategories"] ?? "cpu,gpu,cooler,motherboard,ram,psu,case")
        .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    // App config / environment variables

    [Function("ProductGateway")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "patch", "delete", Route = "products/{category}")]
        HttpRequestData req,
        string category)
    {
        _logger.LogInformation("Function triggered. Category: {Category}, Method: {Method}", category, req.Method);

        if (!IsValidCategory(category))
            return await CreateErrorResponse(req, HttpStatusCode.BadRequest, $"Unknown category '{category}'.");

        var token = await GetAzureAdTokenAsync();
        var apiUrl = $"{_apiBaseUrl}/{category.ToUpper()}";

        HttpResponseMessage apiResponse;

        try
        {
            apiResponse = await ForwardRequestAsync(req, apiUrl, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding request to API");
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, ex.Message);
        }

        return await CreateForwardedResponse(req, apiResponse);
    }

    private bool IsValidCategory(string category) =>
        _allowedCategories.Contains(category.ToLower());

    private async Task<string> GetAzureAdTokenAsync()
    {
        var credential = new DefaultAzureCredential();
        var token = await credential.GetTokenAsync(new TokenRequestContext(new[] { _entraResource }));
        return token.Token;
    }

    private async Task<HttpResponseMessage> ForwardRequestAsync(HttpRequestData req, string url, string token)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return req.Method.ToUpper() switch
        {
            "GET" => await httpClient.GetAsync(url),
            "POST" => await httpClient.PostAsync(url, new StringContent(await req.ReadAsStringAsync(), Encoding.UTF8, "application/json")),
            "PATCH" => await httpClient.PatchAsync(url, new StringContent(await req.ReadAsStringAsync(), Encoding.UTF8, "application/json")),
            "DELETE" => await httpClient.DeleteAsync(url),
            _ => throw new InvalidOperationException($"Unsupported HTTP method {req.Method}")
        };
    }

    private async Task<HttpResponseData> CreateForwardedResponse(HttpRequestData req, HttpResponseMessage apiResponse)
    {
        var content = await apiResponse.Content.ReadAsStringAsync();
        var res = req.CreateResponse(apiResponse.StatusCode);

        res.Headers.Add("Content-Type", apiResponse.Content.Headers.ContentType?.ToString() ?? "application/json; charset=utf-8");
        await res.WriteStringAsync(content, Encoding.UTF8);
        return res;
    }

    private async Task<HttpResponseData> CreateErrorResponse(HttpRequestData req, HttpStatusCode status, string message)
    {
        var res = req.CreateResponse(status);
        await res.WriteStringAsync(message);
        return res;
    }
}
