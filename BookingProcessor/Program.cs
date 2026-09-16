
extern alias AzIdentity;
using DefaultAzureCredential = AzIdentity::Azure.Identity.DefaultAzureCredential;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BookingProcessor.Data;
using BookingProcessor.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        var keyVaultUri = Environment.GetEnvironmentVariable("KeyVaultUri");
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
            config.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        var sqlConnectionString = configuration["SqlConnectionString"]
            ?? throw new InvalidOperationException("SqlConnectionString not found in configuration/Key Vault.");

        services.AddDbContext<BookingDbContext>(options =>
            options.UseSqlServer(sqlConnectionString, sql => sql.EnableRetryOnFailure(3)));

        var catalogApiBaseUrl = configuration["CatalogApiBaseUrl"]
            ?? throw new InvalidOperationException("CatalogApiBaseUrl app setting is missing.");

        services.AddHttpClient<ICatalogApiClient, CatalogApiClient>(client =>
        {
            client.BaseAddress = new Uri(catalogApiBaseUrl);
        });

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
