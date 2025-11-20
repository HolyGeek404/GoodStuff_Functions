using Microsoft.Azure.Functions.Worker.Http;

namespace GoodStuff.Functions.Interfaces;

public interface IFunctionService
{
    Task<HttpResponseMessage> ProcessRequest(HttpRequestData request, string api, string endpoint);
    Task<HttpResponseData> HandleResponse(HttpResponseMessage response, HttpRequestData req);
}