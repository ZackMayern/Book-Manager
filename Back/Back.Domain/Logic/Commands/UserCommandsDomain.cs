using Back.Domain.Abstractions.Dal;

namespace Back.Domain.Logic.Commands;

public sealed class UserCommandsDomain(IUserCommandsRepository commandsRepository) : IUserCommandsDomain
{
    private readonly IUserCommandsRepository _commandsRepository = commandsRepository ?? throw new ArgumentNullException(nameof(commandsRepository));

    public Task<Result> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        return _commandsRepository.UpdateAsync(user, cancellationToken);
    }

    public Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        return _commandsRepository.DeleteAsync(id, cancellationToken);
    }
}