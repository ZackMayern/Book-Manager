namespace Back.Domain.Services.Abstractions;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateRefreshToken();
}