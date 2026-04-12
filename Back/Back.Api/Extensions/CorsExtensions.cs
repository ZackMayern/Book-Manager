using Back.Domain;

namespace Back.Api.Extensions;

public static class CorsExtensions
{
    public static void SetCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(Constants.CorsAllow, builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    public static void EnableCors(this IApplicationBuilder app)
    {
        app.UseCors(Constants.CorsAllow);
    }
}