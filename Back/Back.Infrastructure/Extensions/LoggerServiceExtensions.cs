namespace Back.Infrastructure.Extensions;

public static class LoggerServiceExtensions
{
    public static IServiceCollection AddLoggerService(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerService, LoggerService>();
        return services;
    }
}
