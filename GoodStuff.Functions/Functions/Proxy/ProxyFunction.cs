using System.Net;
using GoodStuff.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GoodStuff.Functions.Functions.Proxy;

public class ProxyFunction(ILogger<ProxyFunction> logger, IFunctionService functionService)
{
    [Function("ApiGateway")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, 
                "GET", "POST", "PATCH", "DELETE", Route = "proxy/{api}/{endpoint}")]
        HttpRequestData req,
        string api,
        string endpoint)
    {
        logger.LogInformation("Function triggered. API: {Api}, Method: {Method}, Endpoint: {Endpoint}", api, req.Method, endpoint);

        try
        {
            var result = await functionService.ProcessRequest(req, api, endpoint);
            var response = await functionService.HandleResponse(result, req);
            
            logger.LogInformation("Function finished successfully . API: {Api}, Method: {Method}, Endpoint: {Endpoint}", api, req.Method, endpoint);
            
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during processing request. API: {Api}, Endpoint: {Endpoint}", api, endpoint);

            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            errorResponse.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            await errorResponse.WriteStringAsync("An unexpected server error occurred.");

            return errorResponse;
        }
    }
}