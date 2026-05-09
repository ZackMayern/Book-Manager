using Back.Domain.Abstractions.Dal;

namespace Back.Domain.Logic.Commands;

public sealed class BorrowCommandsDomain(IBorrowCommandsRepository repository) : IBorrowCommandsDomain
{
    private readonly IBorrowCommandsRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public Task<Result<DatabaseEventType>> AddAsync(BorrowRecord record, CancellationToken cancellationToken = default)
    {
        return _repository.AddAsync(record, cancellationToken);
    }

    public Task<Result<DatabaseEventType>> UpdateAsync(BorrowRecord record, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(record, cancellationToken);
    }
}