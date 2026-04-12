using Back.Api;
using Back.Api.Abstractions;
using Back.Domain;
using Back.Domain.Helpers;
using Back.Domain.Models;
using Back.Domain.Services.Abstractions;
using Back.Models;
using Back.Services;
using Microsoft.Extensions.Options;
using Supabase;
using SupabaseClient = Supabase.Client;

namespace Back.Extensions;

internal static class SupabaseServiceExtensions
{
    public static IServiceCollection AddSupabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        string encryptionKey = configuration.GetSection("AppSettings").GetValue<string>(Constants.EncryptionKey) ??
                               throw new ArgumentNullException(nameof(Constants.EncryptionKey));

        services.Configure<SupabaseSettings>(configuration.GetSection(nameof(SupabaseSettings)));

        services.AddSingleton(sp =>
        {
            SupabaseSettings options = sp.GetRequiredService<IOptions<SupabaseSettings>>().Value;
            options.ConnectionString = CryptoUtility.DecryptString(options.EncryptedConnectionString, encryptionKey);
            options.ApiKey = CryptoUtility.DecryptString(options.EncryptedApiKey, encryptionKey);
            return options;
        });

        services.AddScoped(sp =>
        {
            SupabaseOptions options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };
            SupabaseSettings settings = sp.GetRequiredService<SupabaseSettings>();
            return new SupabaseClient(settings.ConnectionString, settings.ApiKey, options);
        });

        services.AddScoped<ISupabaseClientService, SupabaseClientService>();
        services.AddScoped<IRouterModule, BooksModule>();

        return services;
    }
}
