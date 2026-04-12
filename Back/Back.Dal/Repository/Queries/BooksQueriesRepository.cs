namespace Back.Dal.Repository.Queries;

public sealed class BooksQueriesRepository(ISupabaseClientService supabaseClientService) : IBookQueriesRepository
{
    private readonly ISupabaseClientService _supabaseClientService = supabaseClientService ?? throw new ArgumentNullException(nameof(supabaseClientService));

    public async Task<List<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Book> result = await _supabaseClientService.GetAllAsync<Book>("books", cancellationToken);
        return result;
    }
}