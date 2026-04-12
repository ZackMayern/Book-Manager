namespace Back.Domain.Models;

public class SupabaseSettings
{
    public string EncryptedConnectionString { get; set; } = string.Empty;
    public string EncryptedApiKey { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}