using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using CatalogApi.Configuration;
using CatalogApi.Data;
using CatalogApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection(AzureOptions.SectionName));
var azureOptions = builder.Configuration.GetSection(AzureOptions.SectionName).Get<AzureOptions>()
    ?? throw new InvalidOperationException("Missing 'Azure' configuration section.");

var credential = new DefaultAzureCredential();
builder.Configuration.AddAzureKeyVault(new Uri(azureOptions.KeyVaultUri), credential);

var sqlConnectionString = builder.Configuration[azureOptions.SqlConnectionStringSecretName]
    ?? throw new InvalidOperationException($"Secret '{azureOptions.SqlConnectionStringSecretName}' not found in Key Vault.");

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(sqlConnectionString, sql =>
    {
        sql.EnableRetryOnFailure(3);
        sql.MigrationsHistoryTable("__EFMigrationsHistory_Catalog");
    }));

builder.Services.AddScoped<IBusService, BusService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<ISeatService, SeatService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v1"
    });
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
