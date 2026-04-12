using Back;
using Back.Api;
using Back.Api.Abstractions;
using Back.Domain;
using Back.Domain.Helpers;
using Back.Domain.Models;
using Back.Domain.Services.Abstractions;
using Back.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Back.Extensions;

internal static class MongoServiceExtensions
{
    public static IServiceCollection AddMongoDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        string encryptionKey = configuration.GetSection("AppSettings").GetValue<string>(Constants.EncryptionKey) ??
                               throw new ArgumentNullException(nameof(Constants.EncryptionKey));

        services.Configure<MongoDBSettings>(configuration.GetSection(nameof(MongoDBSettings)));

        services.AddSingleton(sp =>
        {
            MongoDBSettings options = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
            options.ConnectionString = CryptoUtility.DecryptString(options.EncryptedConnectionString, encryptionKey);
            options.DatabaseName = CryptoUtility.DecryptString(options.EncryptedDatabaseName, encryptionKey);
            return options;
        });

        services.AddScoped<IMongoClient, MongoClient>(sp =>
        {
            MongoDBSettings settings = sp.GetRequiredService<MongoDBSettings>();
            return new MongoClient(settings.ConnectionString);
        });

        services.AddScoped<IMongoDBService, MongoDBService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IRouterModule, UserModule>();
        services.AddScoped<IRouterModule, AuthModule>();

        return services;
    }
}
