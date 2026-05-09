namespace Back.Domain.Abstractions.Dal;

public interface IBookRequestQueriesRepository
{
    Task<List<BookRequest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BookRequest?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}