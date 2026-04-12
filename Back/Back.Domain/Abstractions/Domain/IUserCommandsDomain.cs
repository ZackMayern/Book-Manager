namespace Back.Domain.Abstractions.Domain;

public interface IUserCommandsDomain
{
    Task<Result> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
}