using Back.Domain;

namespace Back.Dal.Repository.Commands;

public class UserCommandsRepository(IMongoDbService mongoDbService) : IUserCommandsRepository
{
    private readonly IMongoDbService _mongoDbService = mongoDbService ?? throw new ArgumentNullException(nameof(mongoDbService));

    public async Task<Result> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result = await _mongoDbService.CreateAsync(Constants.Collection.Users, user, cancellationToken);
        return result.IsSuccess ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Message).FirstOrDefault());
    }

    public async Task<Result> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result = await _mongoDbService.UpdateAsync(Constants.Collection.Users, user.Id, user, cancellationToken);
        return result.IsSuccess ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Message).FirstOrDefault());
    }

    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        Result<DatabaseEventType> result = await _mongoDbService.DeleteAsync<User>(Constants.Collection.Users, id, cancellationToken);
        return result.IsSuccess ? Result.Ok() : Result.Fail(result.Errors.Select(e => e.Message).FirstOrDefault());
    }
}