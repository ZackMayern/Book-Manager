namespace Back.Domain.Abstractions.Domain;

public interface IBookRequestCommandsDomain
{
    Task<Result<DatabaseEventType>> AddAsync(BookRequest request, CancellationToken cancellationToken = default);
    Task<Result<DatabaseEventType>> UpdateAsync(BookRequest request, CancellationToken cancellationToken = default);
}