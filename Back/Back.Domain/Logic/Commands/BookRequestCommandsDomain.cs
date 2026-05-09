using Back.Domain.Abstractions.Dal;

namespace Back.Domain.Logic.Commands;

public sealed class BookRequestCommandsDomain(IBookRequestCommandsRepository repository) : IBookRequestCommandsDomain
{
    private readonly IBookRequestCommandsRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public Task<Result<DatabaseEventType>> AddAsync(BookRequest request, CancellationToken cancellationToken = default)
    {
        return _repository.AddAsync(request, cancellationToken);
    }

    public Task<Result<DatabaseEventType>> UpdateAsync(BookRequest request, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateAsync(request, cancellationToken);
    }
}