using System.Net;
using GoodStuff.Functions.Models;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GoodStuff.Functions.Services;

public class FunctionService(
     IOptions<Dictionary<string, ApiRoute>> routes,
     IHttpClientFactory httpClientFactory,
     ILogger<FunctionService> logger,
     IHttpRequestMessageProvider httpRequestMessageProvider)
{
     public async Task<HttpResponseMessage> HandleRequest(HttpRequestData request, string api, string endpoint)
     {
          var apiRoute = routes.Value.FirstOrDefault(x => x.Key == api).Value;
          if (apiRoute == null)
          {
               throw new ArgumentException($"Route {api} not found");
          }

          try
          {
               var httpClient = httpClientFactory.CreateClient( apiRoute.BaseUrl);
               var message = await httpRequestMessageProvider.GetHttpRequestMessage(request, apiRoute, endpoint);
               var response = await httpClient.SendAsync(message);
               return response;
          }
          catch (Exception e)
          {
               Console.WriteLine(e);
               throw;
          }
     }
}