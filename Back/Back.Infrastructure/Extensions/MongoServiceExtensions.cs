using Back.Domain;
using MongoDB.Driver;

namespace Back.Infrastructure.Extensions;

public static class MongoServiceExtensions
{
    public static IServiceCollection AddMongoDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        var encryptionKey = configuration.GetSection("AppSettings").GetValue<string>(Constants.EncryptionKey) ??
                            throw new ArgumentNullException(nameof(Constants.EncryptionKey));

        services.Configure<MongoDBSettings>(configuration.GetSection(nameof(MongoDBSettings)));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
            var connectionString = CryptoUtility.DecryptString(options.EncryptedConnectionString, encryptionKey);
            return new MongoClient(connectionString);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
            var databaseName = CryptoUtility.DecryptString(options.EncryptedDatabaseName, encryptionKey);
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        services.AddScoped<IMongoDbService, MongoDbService>();

        return services;
    }
}
