using Back.Domain.Helpers;

namespace Back.Api.Endpoints;

public class BooksModule : IRouterModule
{
    private const string Url = "api/books";

    public void MapEndpointRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Url}/getAll", GetAsync).RequireAuthorization();
        app.MapPost($"{Url}/add", AddAsync).RequireAuthorization("AdminOnly");
        app.MapPut($"{Url}/update", UpdateAsync).RequireAuthorization("AdminOnly");
        app.MapDelete($"{Url}/delete", DeleteAsync).RequireAuthorization("AdminOnly");
    }

    private static async Task<List<BookDto>> GetAsync(
        [FromServices] IBookQueriesDomain queriesDomain,
        CancellationToken cancellationToken = default)
    {
        var models = await queriesDomain.GetAllBooksAsync(cancellationToken);
        return models.Select(b => b.ToDto()).ToList();
    }

    private static async Task<Result<DatabaseEventType>> AddAsync(
        [FromServices] IBookCommandsDomain commandsDomain,
        [FromBody] BookDto bookDto,
        CancellationToken cancellationToken = default)
    {
        var book = bookDto.ToModel();
        Result<DatabaseEventType> result = await commandsDomain.AddAsync(book, cancellationToken);
        return result;
    }

    private static async Task<Result<DatabaseEventType>> UpdateAsync(
        [FromServices] IBookCommandsDomain commandsDomain,
        [FromBody] BookDto bookDto,
        CancellationToken cancellationToken = default)
    {
        var book = bookDto.ToModel();
        Result<DatabaseEventType> result = await commandsDomain.UpdateAsync(book, cancellationToken);
        return result;
    }

    private static async Task<Result<DatabaseEventType>> DeleteAsync(
        [FromServices] IBookCommandsDomain commandsDomain,
        [FromQuery] string id,
        CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result = await commandsDomain.DeleteAsync(id, cancellationToken);
        return result;
    }
}