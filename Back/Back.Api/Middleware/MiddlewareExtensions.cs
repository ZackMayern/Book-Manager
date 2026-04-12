namespace Back.Api.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();

        return app;
    }
}