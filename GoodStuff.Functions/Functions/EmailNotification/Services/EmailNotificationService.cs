using System.Text.Json;
using GoodStuff.Functions.Interfaces;
using GoodStuff.Functions.Models;
using Microsoft.Azure.Functions.Worker.Http;

namespace GoodStuff.Functions.Functions.EmailNotification.Services;

public class EmailNotificationService(
    IEmailService emailService) : IEmailNotificationService
{
    public async Task ProcessRequest(HttpRequestData req, string type)
    {
        var content = req.ReadAsStringAsync().Result!;

        switch (type)
        {
            case "verification":
            {
                await SendVerificationEmail(content);
                break;
            }
        }
    }

    private async Task SendVerificationEmail(string content)
    {
        var data = JsonSerializer.Deserialize<VerificationRequest>(content);
        await emailService.SendVerificationEmail(data!.Email, data.VerificationKey);
    }
}