using Back.Domain.Services.Abstractions;
using Back.Services;

namespace Back.Extensions;

internal static class LoggerServiceExtensions
{
    public static IServiceCollection AddLoggerService(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerService, LoggerService>();
        return services;
    }
}
