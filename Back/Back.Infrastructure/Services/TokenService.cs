using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Back.Infrastructure.Services;

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public string CreateAccessToken(User user)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(ClaimTypes.NameIdentifier, user.Email),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Email, user.Email),
            new("userId", user.Id)
        ];

        claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

        string jwtKey = _configuration.GetSection(nameof(JwtSettings)).GetValue<string>("Key")!;
        SymmetricSecurityKey key = new(
            Encoding.UTF8.GetBytes(jwtKey)
        );

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        string? issuer = _configuration.GetSection(nameof(JwtSettings)).GetValue<string>("Issuer");
        string? audience = _configuration.GetSection(nameof(JwtSettings)).GetValue<string>("Audience");
        int expiryMinutes = _configuration.GetSection(nameof(JwtSettings)).GetValue<int>("ExpiryMinutes");

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
