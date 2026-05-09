using Back.Domain;

namespace Back.Dal.Repository.Queries;

public sealed class BookRequestQueriesRepository(ISupabaseClientService supabaseClientService) : IBookRequestQueriesRepository
{
    private readonly ISupabaseClientService _supabaseClientService =
        supabaseClientService ?? throw new ArgumentNullException(nameof(supabaseClientService));

    public async Task<List<BookRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _supabaseClientService.GetAllAsync<BookRequest>(Constants.Collection.BookRequests, cancellationToken);
    }

    public async Task<BookRequest?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _supabaseClientService.GetByIdAsync<BookRequest>(Constants.Collection.BookRequests, id, cancellationToken);
    }
}