using Back.Domain.Models.Requests;

namespace Back.Domain.Abstractions.Domain;

public interface IAuthDomain
{
    Task<Result<Token>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<string>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}