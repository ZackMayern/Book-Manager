using Back.Domain.Abstractions.Dal;

namespace Back.Domain.Logic.Queries;

public class BookQueriesDomain(IBookQueriesRepository queriesRepository) : IBookQueriesDomain
{
    public Task<List<Book>> GetAllBooksAsync(CancellationToken cancellationToken = default)
    {
        return queriesRepository.GetAllAsync(cancellationToken);
    }
}