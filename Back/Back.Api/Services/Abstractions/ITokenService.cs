using Back.Models;

namespace Back.Services.Abstractions
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken();
    }
}
