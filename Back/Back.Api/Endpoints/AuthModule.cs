namespace Back.Api.Endpoints;

public sealed class AuthModule : IRouterModule
{
    private const string Url = "api/auth";
    private const string RefreshCookieName = "refreshToken";

    public void MapEndpointRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Url}/login", LoginAsync);
        app.MapPost($"{Url}/signup", RegisterAsync);
        app.MapPost($"{Url}/refresh", RefreshTokenAsync);
        app.MapGet($"{Url}/me", GetCurrentUserAsync).RequireAuthorization();
        app.MapPost($"{Url}/logout", LogoutAsync);
    }

    private async Task<Result<Token>> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] IAuthDomain authDomain,
        [FromServices] IWebHostEnvironment environment,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        Result<Token> result = await authDomain.LoginAsync(request, cancellationToken);
        if (result.IsSuccess && result.ValueOrDefault is { } token)
            SetRefreshCookie(context, token.RefreshToken, environment);

        return result;
    }

    private async Task<Result<string>> RegisterAsync(
        [FromBody] RegisterRequest request,
        [FromServices] IAuthDomain authDomain,
        CancellationToken cancellationToken = default)
    {
        return await authDomain.RegisterAsync(request, cancellationToken);
    }

    private async Task<Result<Token>> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request,
        [FromServices] IAuthDomain authDomain,
        [FromServices] IWebHostEnvironment environment,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        string? refreshToken = context.Request.Cookies[RefreshCookieName];
        Result<Token> result = await authDomain.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = refreshToken ?? request.RefreshToken },
            cancellationToken);

        if (result.IsSuccess && result.ValueOrDefault is { } token)
            SetRefreshCookie(context, token.RefreshToken, environment);

        return result;
    }

    private static async Task<Result<UserDto>> GetCurrentUserAsync(
        HttpContext context,
        [FromServices] IAuthDomain authDomain,
        [FromServices] IWebHostEnvironment environment,
        CancellationToken cancellationToken = default)
    {
        string? email = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail<UserDto>("Authenticated user identity is missing");

        Result<User> result = await authDomain.GetCurrentUserAsync(email, cancellationToken);
        return result.IsFailed
            ? Result.Fail<UserDto>(result.Errors.Select(error => error.Message))
            : Result.Ok(result.Value.ToDto());
    }

    private static async Task<IResult> LogoutAsync(
        HttpContext context,
        [FromServices] IAuthDomain authDomain,
        [FromServices] IWebHostEnvironment environment,
        CancellationToken cancellationToken = default)
    {
        string? refreshToken = context.Request.Cookies[RefreshCookieName];
        Result result = await authDomain.RevokeRefreshTokenAsync(refreshToken ?? string.Empty, cancellationToken);
        context.Response.Cookies.Delete(RefreshCookieName, GetCookieOptions(environment));

        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Errors.Select(error => error.Message));
    }

    private static void SetRefreshCookie(HttpContext context, string refreshToken, IWebHostEnvironment environment)
    {
        context.Response.Cookies.Append(RefreshCookieName, refreshToken, GetCookieOptions(environment));
    }

    private static CookieOptions GetCookieOptions(IWebHostEnvironment environment) => new()
    {
        HttpOnly = true,
        Secure = !environment.IsDevelopment(),
        SameSite = SameSiteMode.Strict,
        IsEssential = true,
        Expires = DateTimeOffset.UtcNow.AddDays(1),
        Path = "/api/auth"
    };
}