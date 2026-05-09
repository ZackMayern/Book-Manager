using Back.Api.Endpoints;

namespace Back.Api.Extensions;

public static class ApiModuleExtensions
{
    public static IServiceCollection AddApiModules(this IServiceCollection services)
    {
        services.AddScoped<IRouterModule, UserModule>();
        services.AddScoped<IRouterModule, AuthModule>();
        services.AddScoped<IRouterModule, BooksModule>();
        services.AddScoped<IRouterModule, BorrowModule>();
        services.AddScoped<IRouterModule, BookRequestsModule>();
        return services;
    }
}
