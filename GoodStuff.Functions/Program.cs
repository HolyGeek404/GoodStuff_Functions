using Azure.Identity;
using GoodStuff.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication().Configuration
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);

var azureAd = builder.Configuration.GetSection("AzureAd");
builder.Configuration.AddAzureKeyVault(new Uri(azureAd["KvUrl"]), new DefaultAzureCredential());
builder.Services.AddLogging(loggingBuilder =>
    loggingBuilder.AddSerilog(new LoggerConfiguration().WriteTo.Console().CreateLogger()));

builder.Services.AddHttpClient();
builder.Services.AddServices();
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();