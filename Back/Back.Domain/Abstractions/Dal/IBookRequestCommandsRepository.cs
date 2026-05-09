namespace Back.Domain.Abstractions.Dal;

public interface IBookRequestCommandsRepository
{
    Task<Result<DatabaseEventType>> AddAsync(BookRequest request, CancellationToken cancellationToken = default);
    Task<Result<DatabaseEventType>> UpdateAsync(BookRequest request, CancellationToken cancellationToken = default);
}