using System.Net;
using Back.Domain.Services.Abstractions;

namespace Back.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILoggerService loggerService, IHostEnvironment environment)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly ILoggerService _loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));
    private readonly IHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _loggerService.LogError(ex, $"Unhandled exception occurred: {ex.Message}");
            await HandleExceptionAsync(context, ex, _environment.IsDevelopment());
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, bool isDevelopment)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = "An internal server error occurred",
            details = exception.Message,
            stackTrace = isDevelopment ? exception.StackTrace : null,
            innerException = isDevelopment ? exception.InnerException?.Message : null
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}

