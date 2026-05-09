namespace Back.Domain.Abstractions.Dal;

public interface IBorrowCommandsRepository
{
    Task<Result<DatabaseEventType>> AddAsync(BorrowRecord record, CancellationToken cancellationToken = default);
    Task<Result<DatabaseEventType>> UpdateAsync(BorrowRecord record, CancellationToken cancellationToken = default);
}