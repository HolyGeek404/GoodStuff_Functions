using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GoodStuff.Functions.Services;

public class FunctionService(
     IValidatorService validatorService,
     IHttpClientFactory httpClientFactory,
     ILogger<FunctionService> logger,
     IHttpRequestMessageProvider httpRequestMessageProvider) : IFunctionService
{
     public async Task<HttpResponseMessage> HandleRequest(HttpRequestData request, string api)
     {
          try
          {
               var apiRoute = validatorService.ValidateApi(api);
               var httpClient = httpClientFactory.CreateClient( apiRoute.BaseUrl);
               var message = await httpRequestMessageProvider.GetHttpRequestMessage(request, apiRoute);
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