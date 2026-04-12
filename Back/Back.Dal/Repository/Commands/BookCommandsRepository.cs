using Back.Domain;

namespace Back.Dal.Repository.Commands;

public class BookCommandsRepository(ISupabaseClientService supabaseClientService) : IBookCommandsRepository
{
    private readonly ISupabaseClientService _supabaseClientService =
        supabaseClientService ?? throw new ArgumentNullException(nameof(supabaseClientService));

    public async Task<Result<DatabaseEventType>> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result =
            await _supabaseClientService.CreateAsync(Constants.Collection.Books, book, cancellationToken);
        return result;
    }

    public async Task<Result<DatabaseEventType>> UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result =
            await _supabaseClientService.UpdateAsync(Constants.Collection.Books, book.Id, book, cancellationToken);
        return result;
    }

    public async Task<Result<DatabaseEventType>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result =
            await _supabaseClientService.DeleteAsync<Book>(Constants.Collection.Books, id, cancellationToken);
        return result;
    }
}