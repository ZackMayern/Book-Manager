namespace Back.Domain.Abstractions.Domain;

public interface IBorrowQueriesDomain
{
    Task<List<BorrowRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BorrowRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}