using Back.Domain.Abstractions.Dal;

namespace Back.Domain.Logic.Queries;

public sealed class BorrowQueriesDomain(IBorrowQueriesRepository repository) : IBorrowQueriesDomain
{
    private readonly IBorrowQueriesRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public Task<List<BorrowRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }

    public Task<BorrowRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }
}