namespace Back.Domain.Abstractions.Domain;

public interface IBookQueriesDomain
{
    Task<List<Book>> GetAllBooksAsync(CancellationToken cancellationToken = default);
}