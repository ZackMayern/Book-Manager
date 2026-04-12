namespace Back.Domain.Models;

public sealed class MongoDBSettings
{
    public string EncryptedConnectionString { get; set; } = string.Empty;
    public string EncryptedDatabaseName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}