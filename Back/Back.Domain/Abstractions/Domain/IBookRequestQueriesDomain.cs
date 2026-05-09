namespace Back.Domain.Abstractions.Domain;

public interface IBookRequestQueriesDomain
{
    Task<List<BookRequest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BookRequest?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}