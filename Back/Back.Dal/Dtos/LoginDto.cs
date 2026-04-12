namespace Back.Dal.Dtos;

public sealed class LoginDto
{
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}