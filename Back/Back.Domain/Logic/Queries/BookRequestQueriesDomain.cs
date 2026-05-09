using Back.Domain.Abstractions.Dal;

namespace Back.Domain.Logic.Queries;

public sealed class BookRequestQueriesDomain(IBookRequestQueriesRepository repository) : IBookRequestQueriesDomain
{
    private readonly IBookRequestQueriesRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public Task<List<BookRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }

    public Task<BookRequest?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }
}