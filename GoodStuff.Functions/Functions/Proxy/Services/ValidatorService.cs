using GoodStuff.Functions.Interfaces;
using GoodStuff.Functions.Models;
using Microsoft.Extensions.Configuration;

namespace GoodStuff.Functions.Functions.Proxy.Services;

public class ValidatorService(IConfiguration configuration) : IValidatorService
{
    public ApiRoute ValidateApi(string api)
    {
        var apiRoutes = configuration.GetSection("ApiRoutes");
        var apiRoute = apiRoutes[$"{api.ToLower()}:BaseUrl"];
        if (apiRoute == null)
            throw new ArgumentException($"Route {api} not found");

        return new ApiRoute
        {
            BaseUrl = apiRoutes[$"{api.ToLower()}:BaseUrl"]!,
            Scope = apiRoutes[$"{api.ToLower()}:Scope"]!
        };
    }
}