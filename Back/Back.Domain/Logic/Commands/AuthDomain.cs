using Back.Domain.Abstractions.Dal;
using Back.Domain.Models.Requests;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Back.Domain.Logic.Commands;

public sealed class AuthDomain(IUserCommandsRepository commandsRepository, IUserQueriesRepository queriesRepository, ITokenService tokenService) : IAuthDomain
{
    private readonly IUserCommandsRepository _commandsRepository = commandsRepository ?? throw new ArgumentNullException(nameof(commandsRepository));
    private readonly IUserQueriesRepository _queriesRepository = queriesRepository ?? throw new ArgumentNullException(nameof(queriesRepository));
    private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

    public async Task<Result<Token>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        User? storedUser = await _queriesRepository.GetByEmailAsync(request.Email, cancellationToken);
            
        if (storedUser == null)
            return Result.Fail("Invalid email or password");
            
        if (!BCryptNet.Verify(request.Password, storedUser.PasswordHash))
            return Result.Fail("Invalid email or password");
            
        string refreshToken = _tokenService.CreateRefreshToken();
        storedUser.RefreshToken = refreshToken;
        storedUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            
        Result updateResult = await _commandsRepository.UpdateAsync(storedUser, cancellationToken);
        if (updateResult.IsFailed)
            return Result.Fail("Failed to update user");
            
        string accessToken = _tokenService.CreateAccessToken(storedUser);
            
        Token token = new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = storedUser
        };
            
        return Result.Ok(token);
    }

    public async Task<Result<string>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result.Fail("Email and password are required");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            return Result.Fail("First Name is required");

        User? existingUser = await _queriesRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
            return Result.Fail("User with this email already exists");

        string hashedPassword = BCryptNet.HashPassword(request.Password);

        User newUser = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = hashedPassword,
            Roles = ["User"]
        };

        Result result = await _commandsRepository.AddAsync(newUser, cancellationToken);
            
        if (result.IsFailed)
            return Result.Fail(result.Errors.Select(e => e.Message).FirstOrDefault() ?? "Failed to register user");

        return Result.Ok("User registered successfully");
    }
}