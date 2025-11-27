using Microsoft.Azure.Functions.Worker.Http;

namespace GoodStuff.Functions.Interfaces;

public interface IEmailNotificationService
{
    Task ProcessRequest(HttpRequestData req, string type);
}