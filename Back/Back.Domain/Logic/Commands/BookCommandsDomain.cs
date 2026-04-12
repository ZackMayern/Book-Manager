using Back.Domain.Abstractions.Dal;

namespace Back.Domain.Logic.Commands;

public sealed class BookCommandsDomain(IBookCommandsRepository repository) : IBookCommandsDomain
{
    private readonly IBookCommandsRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    public async Task<Result<DatabaseEventType>> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        var result = await _repository.AddAsync(book, cancellationToken);
        return result;
    }

    public Task<Result<DatabaseEventType>> UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        var result = _repository.UpdateAsync(book, cancellationToken);
        return result;
    }

    public Task<Result<DatabaseEventType>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = _repository.DeleteAsync(id, cancellationToken);
        return result;
    }
}