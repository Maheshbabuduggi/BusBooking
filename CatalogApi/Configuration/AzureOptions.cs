namespace CatalogApi.Configuration;

public class AzureOptions
{
    public const string SectionName = "Azure";
    public string KeyVaultUri { get; set; } = string.Empty;
    public string SqlConnectionStringSecretName { get; set; } = "SqlConnectionString";
}
