namespace Back.Api.Endpoints;

public sealed class AuthModule : IRouterModule
{
    private const string Url = "api/auth";

    public void MapEndpointRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Url}/login", LoginAsync);
        app.MapPost($"{Url}/signup", RegisterAsync);
    }

    private async Task<Result<Token>> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] IAuthDomain authDomain,
        CancellationToken cancellationToken = default)
    {
        return await authDomain.LoginAsync(request, cancellationToken);
    }

    private async Task<Result<string>> RegisterAsync(
        [FromBody] RegisterRequest request,
        [FromServices] IAuthDomain authDomain,
        CancellationToken cancellationToken = default)
    {
        return await authDomain.RegisterAsync(request, cancellationToken);
    }
}