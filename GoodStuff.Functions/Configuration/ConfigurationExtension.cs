using GoodStuff.Functions.Functions.EmailNotification.Services;
using GoodStuff.Functions.Functions.Proxy.Services;
using GoodStuff.Functions.Interfaces;
using GoodStuff.Functions.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace GoodStuff.Functions.Configuration;

public static class ConfigurationExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFunctionService, ProxyService>();
        services.AddScoped<IHttpRequestMessageProvider, HttpRequestMessageProvider>();
        services.AddScoped<ITokenProviderService, TokenProviderService>();
        services.AddScoped<IValidatorService, ValidatorService>();
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}