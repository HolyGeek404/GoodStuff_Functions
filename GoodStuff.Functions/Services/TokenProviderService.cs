using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace GoodStuff.Functions.Services;

public class TokenProviderService(IConfiguration configuration) : ITokenProviderService
{
    public async Task<string> GetAccessToken(string scope)
    {
        var azure = configuration.GetSection("AzureAd");
        var tenantId = azure["TenantId"];
        var clientId = azure["ClientId"];
        var clientSecret = azure["ClientSecret"];
        var instanceUrl = azure["Instance"];
        var authority = $"{instanceUrl}{tenantId}";

        var app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority(new Uri(authority))
            .Build();

        var result = await app.AcquireTokenForClient([scope]).ExecuteAsync();

        return result.AccessToken;
    }
}