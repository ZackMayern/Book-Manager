namespace Back.Domain.Abstractions.Domain;

public interface IBorrowCommandsDomain
{
    Task<Result<DatabaseEventType>> AddAsync(BorrowRecord record, CancellationToken cancellationToken = default);
    Task<Result<DatabaseEventType>> UpdateAsync(BorrowRecord record, CancellationToken cancellationToken = default);
}