using Back.Domain;
using Supabase;
using SupabaseClient = Supabase.Client;

namespace Back.Infrastructure.Extensions;

public static class SupabaseServiceExtensions
{
    public static IServiceCollection AddSupabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var encryptionKey = configuration.GetSection("AppSettings").GetValue<string>(Constants.EncryptionKey) ??
                            throw new ArgumentNullException(nameof(Constants.EncryptionKey));

        services.Configure<SupabaseSettings>(configuration.GetSection(nameof(SupabaseSettings)));

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SupabaseSettings>>().Value;
            options.ConnectionString = CryptoUtility.DecryptString(options.EncryptedConnectionString, encryptionKey);
            options.ApiKey = CryptoUtility.DecryptString(options.EncryptedApiKey, encryptionKey);
            return options;
        });

        services.AddScoped(sp =>
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };
            var settings = sp.GetRequiredService<SupabaseSettings>();
            return new SupabaseClient(settings.ConnectionString, settings.ApiKey, options);
        });

        services.AddScoped<ISupabaseClientService, SupabaseClientService>();

        return services;
    }
}
