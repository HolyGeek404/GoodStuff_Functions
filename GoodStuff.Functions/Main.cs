using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using GoodStuff.Functions.Services;

namespace GoodStuff.Functions;

public class Main(ILogger logger, IConfiguration config)
{
    
    [Function("ApiGateway")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", "POST", "PATCH", "DELETE", Route = "proxy/{api}/{endpoint}")]
        HttpRequestData req,
        string api,
        string endpoint)
    {
        logger.LogInformation("Function triggered. API: {Api}, Category: {Category}, Method: {Method}", api, endpoint, req.Method);

       
        
        
        
        return req.CreateResponse(HttpStatusCode.OK);
    }
}