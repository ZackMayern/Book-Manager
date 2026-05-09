namespace Back.Domain.Abstractions.Domain;

public interface IUserQueriesDomain
{
    Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<List<User>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
}