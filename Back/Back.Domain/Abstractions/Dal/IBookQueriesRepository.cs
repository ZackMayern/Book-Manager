namespace Back.Domain.Abstractions.Dal;

public interface IBookQueriesRepository
{
    Task<List<Book>> GetAllAsync(CancellationToken cancellationToken = default);
}
