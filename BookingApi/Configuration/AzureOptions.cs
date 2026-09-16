namespace BookingApi.Configuration;

public class AzureOptions
{
    public const string SectionName = "Azure";
    public string KeyVaultUri { get; set; } = string.Empty;
    public string SqlConnectionStringSecretName { get; set; } = "SqlConnectionString";
    public string EventHubFullyQualifiedNamespace { get; set; } = string.Empty;
    public string EventHubName { get; set; } = string.Empty;
    public string CatalogApiBaseUrl { get; set; } = string.Empty;
}
