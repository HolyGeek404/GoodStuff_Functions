using GoodStuff.Functions.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace GoodStuff.Functions.Functions.EmailNotification;

public class EmailNotificationFunction(IEmailNotificationService service)
{
    [Function("EmailNotification")]

    public async Task<bool> Run([HttpTrigger(AuthorizationLevel.Anonymous,
            "POST", Route = "notification/{type}")] HttpRequestData req, string type)
    {
        await service.ProcessRequest(req, type);
        return true;
    }
    
    
}