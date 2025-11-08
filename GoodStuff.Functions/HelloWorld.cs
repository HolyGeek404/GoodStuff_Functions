using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GoodStuff.Functions;

public class HelloWorld(ILogger<HelloWorld> logger)
{
    [Function("HelloWorld")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        logger.LogInformation("Function triggered: fetching CPUs from API...");

        // 1️⃣ Get Azure AD token using Managed Identity or local credentials
        var credential = new DefaultAzureCredential();

        var token = await credential.GetTokenAsync(
            new TokenRequestContext(["api://56b1c593-a584-4622-b223-bcf0fb117cb1/.default"])
        );

        // 2️⃣ Call your API (secured with Entra ID)
        var apiUrl = "https://localhost:7003/Product/CPU";
        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.Token);

        var response = await httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("API call failed with status {StatusCode}: {Error}",
                response.StatusCode, error);

            var errorRes = req.CreateResponse(response.StatusCode);
            await errorRes.WriteStringAsync($"API call failed: {error}");
            return errorRes;
        }

        // 3️⃣ Read API response and return JSON to client (Angular)
        var content = await response.Content.ReadAsStringAsync();

        var res = req.CreateResponse(HttpStatusCode.OK);
        res.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await res.WriteStringAsync(content, Encoding.UTF8);

        logger.LogInformation("Returning CPU data successfully.");
        return res;
    }
}