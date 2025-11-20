using GoodStuff.Functions.Models;
using Microsoft.Azure.Functions.Worker.Http;

namespace GoodStuff.Functions.Interfaces;

public interface IHttpRequestMessageProvider
{
    Task<HttpRequestMessage> GetHttpRequestMessage(HttpRequestData request, ApiRoute apiRoute, string endpoint);
}