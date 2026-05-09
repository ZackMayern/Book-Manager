namespace Back.Domain.Abstractions.Dal;

public interface IBorrowQueriesRepository
{
    Task<List<BorrowRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BorrowRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}