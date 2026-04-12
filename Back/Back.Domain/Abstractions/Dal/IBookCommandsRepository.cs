namespace Back.Domain.Abstractions.Dal;

public interface IBookCommandsRepository
{
    Task<Result<DatabaseEventType>> AddAsync(Book book, CancellationToken cancellationToken = default);
    Task<Result<DatabaseEventType>> UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task<Result<DatabaseEventType>> DeleteAsync(string id, CancellationToken cancellationToken = default);
}