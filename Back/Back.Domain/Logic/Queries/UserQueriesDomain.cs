using Back.Domain.Abstractions.Dal;

namespace Back.Domain.Logic.Queries;

public sealed class UserQueriesDomain(IUserQueriesRepository queriesRepository) : IUserQueriesDomain
{
    private readonly IUserQueriesRepository _queriesRepository = queriesRepository ?? throw new ArgumentNullException(nameof(queriesRepository));

    public async Task<Result<List<User>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            List<User> users = await _queriesRepository.GetAllAsync(cancellationToken);
            return Result.Ok(users);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _queriesRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _queriesRepository.GetByEmailAsync(email, cancellationToken);
    }
}