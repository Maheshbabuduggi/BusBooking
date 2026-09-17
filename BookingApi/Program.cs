extern alias AzIdentity;
using DefaultAzureCredential = AzIdentity::Azure.Identity.DefaultAzureCredential;

using Azure.Messaging.EventHubs.Producer;
using Microsoft.EntityFrameworkCore;
using BookingApi.Configuration;
using BookingApi.Data;
using BookingApi.Messaging;
using BookingApi.Services;
using Microsoft.OpenApi.Models;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection(AzureOptions.SectionName));
var azureOptions = builder.Configuration.GetSection(AzureOptions.SectionName).Get<AzureOptions>()
    ?? throw new InvalidOperationException("Missing 'Azure' configuration section.");

var credential = new DefaultAzureCredential();
builder.Configuration.AddAzureKeyVault(new Uri(azureOptions.KeyVaultUri), credential);

var sqlConnectionString = builder.Configuration[azureOptions.SqlConnectionStringSecretName]
    ?? throw new InvalidOperationException($"Secret '{azureOptions.SqlConnectionStringSecretName}' not found in Key Vault.");

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(sqlConnectionString, sql =>
    {
        sql.EnableRetryOnFailure(3);
        sql.MigrationsHistoryTable("__EFMigrationsHistory_Booking");
    }));

builder.Services.AddSingleton(_ => new EventHubProducerClient(
    azureOptions.EventHubFullyQualifiedNamespace, azureOptions.EventHubName, credential));

builder.Services.AddHttpClient<ICatalogApiClient, CatalogApiClient>(client =>
{
    client.BaseAddress = new Uri(azureOptions.CatalogApiBaseUrl);
});

builder.Services.AddScoped<IEventPublisher, EventHubPublisher>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Booking API",
        Version = "v1"
    });
});
// Add this near the other builder.Services.Add... calls:
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
