using System.ComponentModel.DataAnnotations;

namespace Back.Api.Endpoints;

public class UserModule : IRouterModule
{
    private const string Url = "api/users";
    public void MapEndpointRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Url}/get", GetAsync).RequireAuthorization("AdminOnly");
        app.MapPut($"{Url}/update", UpdateAsync).RequireAuthorization();
        app.MapDelete($"{Url}/delete", DeleteAsync).RequireAuthorization("AdminOnly");
    }
        
    private async Task<Results<Ok<List<UserDto>>, BadRequest<string>>> GetAsync(
        [FromServices] IUserQueriesDomain queriesDomain,
        CancellationToken cancellationToken = default)
    {
        Result<List<User>> result = await queriesDomain.GetAllUsersAsync(cancellationToken);
        if (result.IsFailed)
            return TypedResults.BadRequest(result.Errors.Select(e => e.Message).FirstOrDefault());

        List<UserDto> users = result.Value.Select(u => u.ToDto()).ToList();
        return TypedResults.Ok(users);
    }

    private async Task<Results<Ok, BadRequest<string>>> UpdateAsync(
        [FromBody] UpdateUserRequest request,
        [FromServices] IUserCommandsDomain commandsDomain,
        [FromServices] IUserQueriesDomain queriesDomain,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
            return TypedResults.BadRequest("User ID is required");

        User? existingUser = await queriesDomain.GetUserByIdAsync(request.Id, cancellationToken);
        if (existingUser == null)
            return TypedResults.BadRequest("User not found");

        existingUser.FirstName = request.FirstName;
        existingUser.LastName = request.LastName;
        existingUser.Email = request.Email;

        Result result = await commandsDomain.UpdateAsync(existingUser, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors.Select(e => e.Message).FirstOrDefault());
    }

    private async Task<Results<Ok, BadRequest<string>>> DeleteAsync(
        [Required] string id,
        [FromServices] IUserCommandsDomain commandsDomain,
        CancellationToken cancellationToken = default)
    {
        Result result = await commandsDomain.DeleteAsync(id, cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Errors.Select(e => e.Message).FirstOrDefault());
    }
}