using Microsoft.Extensions.Primitives;

namespace Back.Api.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        StringValues correlationId = context.Request.Headers.FirstOrDefault(h => h.Key == CorrelationIdHeaderName).Value;

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Append(CorrelationIdHeaderName, correlationId.ToString());

        await _next(context);
    }
}