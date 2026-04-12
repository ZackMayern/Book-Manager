using Back.Domain;

namespace Back.Dal.Repository.Queries;

public class UserQueriesRepository(IMongoDbService mongoDbService) : IUserQueriesRepository
{
    private readonly IMongoDbService _mongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<User> result = await _mongoDbService.GetAsync<User>(Constants.Collection.Users, cancellationToken);
        return result;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        User? result = await _mongoDbService.GetByIdAsync<User>(Constants.Collection.Users, id, cancellationToken);
        return result;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        User? result = await _mongoDbService.GetByEmailAsync<User>(Constants.Collection.Users, email, cancellationToken);
        return result;
    }
}