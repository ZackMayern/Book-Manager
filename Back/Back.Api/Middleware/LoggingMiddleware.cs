using System.Diagnostics;
using Back.Domain.Services.Abstractions;

namespace Back.Api.Middleware;

public class LoggingMiddleware(RequestDelegate next, ILoggerService loggerService)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly ILoggerService _loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));

    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        string method = context.Request.Method;
        PathString path = context.Request.Path;
        string endpoint = $"{method} {path}";

        _loggerService.LogInformation($"[START]: {endpoint} - Request started");

        Stream originalBodyStream = context.Response.Body;

        using (MemoryStream responseBody = new())
        {
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                responseBody.Position = 0;
                await responseBody.CopyToAsync(originalBodyStream);

                stopwatch.Stop();
                int statusCode = context.Response.StatusCode;
                _loggerService.LogInformation($"[END]: {endpoint} - Completed in {stopwatch.ElapsedMilliseconds}ms with status {statusCode}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _loggerService.LogError(ex, $"[ERROR]: {endpoint} failed after {stopwatch.ElapsedMilliseconds}ms");
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}