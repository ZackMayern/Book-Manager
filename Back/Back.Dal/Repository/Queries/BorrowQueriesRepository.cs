using Back.Domain;

namespace Back.Dal.Repository.Queries;

public sealed class BorrowQueriesRepository(ISupabaseClientService supabaseClientService) : IBorrowQueriesRepository
{
    private readonly ISupabaseClientService _supabaseClientService =
        supabaseClientService ?? throw new ArgumentNullException(nameof(supabaseClientService));

    public async Task<List<BorrowRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _supabaseClientService.GetAllAsync<BorrowRecord>(Constants.Collection.BorrowRecords, cancellationToken);
    }

    public async Task<BorrowRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _supabaseClientService.GetByIdAsync<BorrowRecord>(Constants.Collection.BorrowRecords, id, cancellationToken);
    }
}