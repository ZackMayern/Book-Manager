using System.Security.Claims;
using Back.Domain.Helpers;

namespace Back.Api.Endpoints;

public sealed class BorrowModule : IRouterModule
{
    private const string Url = "api/borrow";

    public void MapEndpointRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Url}", BorrowAsync).RequireAuthorization();
        app.MapPut($"{Url}/return/{{id}}", ReturnAsync).RequireAuthorization();
        app.MapPut($"{Url}/renew/{{id}}", RenewAsync).RequireAuthorization();
        app.MapGet($"{Url}/mine", GetMineAsync).RequireAuthorization();
        app.MapGet($"{Url}/all", GetAllAsync).RequireAuthorization("AdminOnly");
    }

    private static async Task<Results<Ok<BorrowRecordDto>, BadRequest<string>>> BorrowAsync(
        HttpContext context,
        [FromBody] BorrowBookRequest request,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        [FromServices] IBookQueriesDomain bookQueriesDomain,
        [FromServices] IBookCommandsDomain bookCommandsDomain,
        [FromServices] IBorrowCommandsDomain borrowCommandsDomain,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.BookId))
            return TypedResults.BadRequest("Book id is required");

        string? email = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue(ClaimTypes.Email)
            ?? context.User.FindFirstValue(ClaimTypes.Name)
            ?? context.User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(email))
            return TypedResults.BadRequest("User is not authenticated");

        User? user = await userQueriesDomain.GetUserByEmailAsync(email, cancellationToken);
        if (user == null)
            return TypedResults.BadRequest("User was not found");

        List<Book> books = await bookQueriesDomain.GetAllBooksAsync(cancellationToken);
        Book? book = books.FirstOrDefault(b => b.Id == request.BookId);

        if (book == null)
            return TypedResults.BadRequest("Book was not found");

        if (book.BookCount <= 0)
            return TypedResults.BadRequest("Book is not available");

        book.BookCount--;
        Result<DatabaseEventType> updateBookResult = await bookCommandsDomain.UpdateAsync(book, cancellationToken);
        if (updateBookResult.IsFailed)
            return TypedResults.BadRequest(updateBookResult.Errors.Select(e => e.Message).FirstOrDefault() ?? "Failed to update book");

        int borrowDays = request.BorrowDays <= 0 ? 14 : request.BorrowDays;
        BorrowRecord borrowRecord = new()
        {
            UserId = user.Id,
            BookId = request.BookId,
            BorrowedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(borrowDays),
            Status = "Active",
            RenewCount = 0
        };

        Result<DatabaseEventType> addResult = await borrowCommandsDomain.AddAsync(borrowRecord, cancellationToken);
        if (addResult.IsFailed)
            return TypedResults.BadRequest(addResult.Errors.Select(e => e.Message).FirstOrDefault() ?? "Failed to create borrow record");

        return TypedResults.Ok(borrowRecord.ToDto(user, book));
    }

    private static async Task<Results<Ok<BorrowRecordDto>, BadRequest<string>>> ReturnAsync(
        [FromRoute] string id,
        [FromServices] IBorrowQueriesDomain borrowQueriesDomain,
        [FromServices] IBorrowCommandsDomain borrowCommandsDomain,
        [FromServices] IBookQueriesDomain bookQueriesDomain,
        [FromServices] IBookCommandsDomain bookCommandsDomain,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        CancellationToken cancellationToken = default)
    {
        BorrowRecord? record = await borrowQueriesDomain.GetByIdAsync(id, cancellationToken);
        if (record == null)
            return TypedResults.BadRequest("Borrow record was not found");

        if (record.Status == "Returned")
            return TypedResults.BadRequest("Book already returned");

        List<Book> books = await bookQueriesDomain.GetAllBooksAsync(cancellationToken);
        Book? book = books.FirstOrDefault(b => b.Id == record.BookId);

        if (book == null)
            return TypedResults.BadRequest("Book was not found");

        book.BookCount++;
        Result<DatabaseEventType> updateBookResult = await bookCommandsDomain.UpdateAsync(book, cancellationToken);
        if (updateBookResult.IsFailed)
            return TypedResults.BadRequest(updateBookResult.Errors.Select(e => e.Message).FirstOrDefault() ?? "Failed to update book");

        record.Status = "Returned";
        record.ReturnedAt = DateTime.UtcNow;
        Result<DatabaseEventType> updateRecordResult = await borrowCommandsDomain.UpdateAsync(record, cancellationToken);
        if (updateRecordResult.IsFailed)
            return TypedResults.BadRequest(updateRecordResult.Errors.Select(e => e.Message).FirstOrDefault() ?? "Failed to update borrow record");

        User? user = await userQueriesDomain.GetUserByIdAsync(record.UserId, cancellationToken);
        return TypedResults.Ok(record.ToDto(user, book));
    }

    private static async Task<Results<Ok<BorrowRecordDto>, BadRequest<string>>> RenewAsync(
        [FromRoute] string id,
        [FromBody] RenewBorrowRequest request,
        [FromServices] IBorrowQueriesDomain borrowQueriesDomain,
        [FromServices] IBorrowCommandsDomain borrowCommandsDomain,
        [FromServices] IBookQueriesDomain bookQueriesDomain,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        CancellationToken cancellationToken = default)
    {
        BorrowRecord? record = await borrowQueriesDomain.GetByIdAsync(id, cancellationToken);
        if (record == null)
            return TypedResults.BadRequest("Borrow record was not found");

        if (record.Status == "Returned")
            return TypedResults.BadRequest("Cannot renew returned borrow");

        if (record.RenewCount >= 3)
            return TypedResults.BadRequest("Maximum renewal count reached");

        int extendDays = request.ExtendDays <= 0 ? 7 : request.ExtendDays;
        record.DueDate = record.DueDate.AddDays(extendDays);
        record.RenewCount++;
        record.Status = "Renewed";

        Result<DatabaseEventType> updateResult = await borrowCommandsDomain.UpdateAsync(record, cancellationToken);
        if (updateResult.IsFailed)
            return TypedResults.BadRequest(updateResult.Errors.Select(e => e.Message).FirstOrDefault() ?? "Failed to renew borrow");

        List<Book> books = await bookQueriesDomain.GetAllBooksAsync(cancellationToken);
        Book? book = books.FirstOrDefault(b => b.Id == record.BookId);
        User? user = await userQueriesDomain.GetUserByIdAsync(record.UserId, cancellationToken);

        return TypedResults.Ok(record.ToDto(user, book));
    }

    private static async Task<Ok<List<BorrowRecordDto>>> GetMineAsync(
        HttpContext context,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        [FromServices] IBorrowQueriesDomain borrowQueriesDomain,
        [FromServices] IBookQueriesDomain bookQueriesDomain,
        CancellationToken cancellationToken = default)
    {
        string? email = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue(ClaimTypes.Email)
            ?? context.User.FindFirstValue(ClaimTypes.Name)
            ?? context.User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(email))
            return TypedResults.Ok(new List<BorrowRecordDto>());

        User? user = await userQueriesDomain.GetUserByEmailAsync(email, cancellationToken);
        if (user == null)
            return TypedResults.Ok(new List<BorrowRecordDto>());

        List<BorrowRecord> records = await borrowQueriesDomain.GetAllAsync(cancellationToken);
        List<Book> books = await bookQueriesDomain.GetAllBooksAsync(cancellationToken);

        List<BorrowRecordDto> result = records
            .Where(r => r.UserId == user.Id)
            .Select(r => r.ToDto(user, books.FirstOrDefault(b => b.Id == r.BookId)))
            .OrderByDescending(r => r.BorrowedAt)
            .ToList();

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<List<BorrowRecordDto>>> GetAllAsync(
        [FromServices] IBorrowQueriesDomain borrowQueriesDomain,
        [FromServices] IUserQueriesDomain userQueriesDomain,
        [FromServices] IBookQueriesDomain bookQueriesDomain,
        CancellationToken cancellationToken = default)
    {
        List<BorrowRecord> records = await borrowQueriesDomain.GetAllAsync(cancellationToken);
        List<User> users = (await userQueriesDomain.GetAllUsersAsync(cancellationToken)).ValueOrDefault ?? [];
        List<Book> books = await bookQueriesDomain.GetAllBooksAsync(cancellationToken);

        List<BorrowRecordDto> result = records
            .OrderByDescending(r => r.BorrowedAt)
            .Select(r => r.ToDto(users.FirstOrDefault(u => u.Id == r.UserId), books.FirstOrDefault(b => b.Id == r.BookId)))
            .ToList();

        return TypedResults.Ok(result);
    }
}