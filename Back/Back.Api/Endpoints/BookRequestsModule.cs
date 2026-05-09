using Back.Domain.Helpers;
using System.Security.Claims;

namespace Back.Api.Endpoints;

public sealed class BookRequestsModule : IRouterModule
{
    private const string Url = "api/requests";

    public void MapEndpointRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(Url, CreateAsync).RequireAuthorization();
        app.MapGet($"{Url}/mine", GetMineAsync).RequireAuthorization();
        app.MapGet($"{Url}/pending", GetPendingAsync).RequireAuthorization("AdminOnly");
        app.MapPut($"{Url}/{{id}}/approve", ApproveAsync).RequireAuthorization("AdminOnly");
        app.MapPut($"{Url}/{{id}}/reject", RejectAsync).RequireAuthorization("AdminOnly");
    }

    private static async Task<Results<Ok<BookRequestDto>, BadRequest<string>>> CreateAsync(
        HttpContext context,
        [FromBody] CreateBookRequest request,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        [FromServices] IBookRequestCommandsDomain requestCommandsDomain,
        CancellationToken cancellationToken = default)
    {
        string? email = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue(ClaimTypes.Email)
            ?? context.User.FindFirstValue(ClaimTypes.Name)
            ?? context.User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(email))
            return TypedResults.BadRequest("User is not authenticated");

        User? user = await userQueriesDomain.GetUserByEmailAsync(email, cancellationToken);
        if (user == null)
            return TypedResults.BadRequest("User was not found");

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Author))
            return TypedResults.BadRequest("Title and author are required");

        BookRequest model = new()
        {
            UserId = user.Id,
            Title = request.Title,
            Author = request.Author,
            Reason = request.Reason,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        Result<DatabaseEventType> result = await requestCommandsDomain.AddAsync(model, cancellationToken);
        if (result.IsFailed)
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message).FirstOrDefault() ?? "Failed to create request");

        return TypedResults.Ok(model.ToDto(user));
    }

    private static async Task<Ok<List<BookRequestDto>>> GetMineAsync(
        HttpContext context,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        [FromServices] IBookRequestQueriesDomain requestQueriesDomain,
        CancellationToken cancellationToken = default)
    {
        string? email = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue(ClaimTypes.Email)
            ?? context.User.FindFirstValue(ClaimTypes.Name)
            ?? context.User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(email))
            return TypedResults.Ok(new List<BookRequestDto>());

        User? user = await userQueriesDomain.GetUserByEmailAsync(email, cancellationToken);
        if (user == null)
            return TypedResults.Ok(new List<BookRequestDto>());

        List<BookRequest> requests = await requestQueriesDomain.GetAllAsync(cancellationToken);
        List<BookRequestDto> result = requests
            .Where(r => r.UserId == user.Id)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.ToDto(user))
            .ToList();

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<BookRequestDto>>> GetPendingAsync(
        [FromServices] IBookRequestQueriesDomain requestQueriesDomain,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        CancellationToken cancellationToken = default)
    {
        List<BookRequest> requests = await requestQueriesDomain.GetAllAsync(cancellationToken);
        List<User> users = (await userQueriesDomain.GetAllUsersAsync(cancellationToken)).ValueOrDefault ?? [];

        List<BookRequestDto> result = requests
            .Where(r => r.Status == "Pending")
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.ToDto(users.FirstOrDefault(u => u.Id == r.UserId)))
            .ToList();

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<BookRequestDto>, BadRequest<string>>> ApproveAsync(
        [FromRoute] string id,
        [FromBody] BookRequestDecisionRequest request,
        [FromServices] IBookRequestQueriesDomain requestQueriesDomain,
        [FromServices] IBookRequestCommandsDomain requestCommandsDomain,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        CancellationToken cancellationToken = default)
    {
        return await DecideAsync(id, "Approved", request, requestQueriesDomain, requestCommandsDomain, userQueriesDomain, cancellationToken);
    }

    private static async Task<Results<Ok<BookRequestDto>, BadRequest<string>>> RejectAsync(
        [FromRoute] string id,
        [FromBody] BookRequestDecisionRequest request,
        [FromServices] IBookRequestQueriesDomain requestQueriesDomain,
        [FromServices] IBookRequestCommandsDomain requestCommandsDomain,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        CancellationToken cancellationToken = default)
    {
        return await DecideAsync(id, "Rejected", request, requestQueriesDomain, requestCommandsDomain, userQueriesDomain, cancellationToken);
    }

    private static async Task<Results<Ok<BookRequestDto>, BadRequest<string>>> DecideAsync(
        string id,
        string decision,
        BookRequestDecisionRequest request,
        IBookRequestQueriesDomain requestQueriesDomain,
        IBookRequestCommandsDomain requestCommandsDomain,
        IUserQueriesDomain userQueriesDomain,
        CancellationToken cancellationToken)
    {
        BookRequest? model = await requestQueriesDomain.GetByIdAsync(id, cancellationToken);
        if (model == null)
            return TypedResults.BadRequest("Request was not found");

        if (model.Status != "Pending")
            return TypedResults.BadRequest("Request has already been processed");

        model.Status = decision;
        model.AdminMessage = request.Message;

        Result<DatabaseEventType> result = await requestCommandsDomain.UpdateAsync(model, cancellationToken);
        if (result.IsFailed)
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message).FirstOrDefault() ?? "Failed to update request");

        User? user = await userQueriesDomain.GetUserByIdAsync(model.UserId, cancellationToken);
        return TypedResults.Ok(model.ToDto(user));
    }
}